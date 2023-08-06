using CSharpFunctionalExtensions;
using Interview.Domain.Events;
using Interview.Domain.Questions;
using Interview.Domain.Reactions;
using Interview.Domain.Repository;
using Interview.Domain.RoomParticipants;
using Interview.Domain.RoomQuestionReactions;
using Interview.Domain.RoomQuestionReactions.Mappers;
using Interview.Domain.RoomQuestionReactions.Specifications;
using Interview.Domain.RoomQuestions;
using Interview.Domain.Rooms.Service.Records.Request;
using Interview.Domain.Rooms.Service.Records.Response;
using Interview.Domain.Rooms.Service.Records.Response.RoomStates;
using Interview.Domain.ServiceResults.Errors;
using Interview.Domain.ServiceResults.Success;
using Interview.Domain.Users;
using NSpecifications;
using Entity = Interview.Domain.Repository.Entity;

namespace Interview.Domain.Rooms.Service;

public sealed class RoomService
{
    private readonly IAppEventRepository _eventRepository;
    private readonly IRoomRepository _roomRepository;
    private readonly IRoomQuestionRepository _roomQuestionRepository;
    private readonly IQuestionRepository _questionRepository;
    private readonly IUserRepository _userRepository;
    private readonly IRoomEventDispatcher _roomEventDispatcher;
    private readonly IRoomQuestionReactionRepository _roomQuestionReactionRepository;

    public RoomService(
        IRoomRepository roomRepository,
        IRoomQuestionRepository roomQuestionRepository,
        IQuestionRepository questionRepository,
        IUserRepository userRepository,
        IRoomEventDispatcher roomEventDispatcher,
        IRoomQuestionReactionRepository roomQuestionReactionRepository,
        IAppEventRepository eventRepository)
    {
        _roomRepository = roomRepository;
        _questionRepository = questionRepository;
        _userRepository = userRepository;
        _roomEventDispatcher = roomEventDispatcher;
        _roomQuestionReactionRepository = roomQuestionReactionRepository;
        _eventRepository = eventRepository;
        _roomQuestionRepository = roomQuestionRepository;
    }

    public async Task<Result<ServiceResult<Room>, ServiceError>> CreateAsync(
        RoomCreateRequest request,
        CancellationToken cancellationToken = default)
    {
        if (request == null)
        {
            throw new ArgumentNullException(nameof(request));
        }

        var name = request.Name.Trim();
        if (string.IsNullOrEmpty(name))
        {
            return ServiceError.Error("Room name should not be empty");
        }

        var questions = await FindByIdsOrErrorAsync(_questionRepository, request.Questions, "questions", cancellationToken);
        if (questions.IsFailure)
        {
            return questions.Error;
        }

        var experts = await FindByIdsOrErrorAsync(_userRepository, request.Experts, "experts", cancellationToken);
        if (experts.IsFailure)
        {
            return experts.Error;
        }

        var examinees = await FindByIdsOrErrorAsync(_userRepository, request.Examinees, "examinees", cancellationToken);
        if (examinees.IsFailure)
        {
            return examinees.Error;
        }

        var twitchChannel = request.TwitchChannel?.Trim();
        if (string.IsNullOrEmpty(twitchChannel))
        {
            return ServiceError.Error($"Twitch channel should not be empty");
        }

        var room = new Room(name, twitchChannel);
        var roomQuestions = questions.Value.Select(question =>
            new RoomQuestion { Room = room, Question = question, State = RoomQuestionState.Open });

        room.Questions.AddRange(roomQuestions);

        var participantsExperts = experts.Value
            .Select(user => new RoomParticipant(user, room, RoomParticipantType.Expert))
            .ToList();

        var participantsExaminees = examinees.Value
            .Select(user => new RoomParticipant(user, room, RoomParticipantType.Examinee))
            .ToList();

        room.Participants.AddRange(participantsExperts);
        room.Participants.AddRange(participantsExaminees);
        await _roomRepository.CreateAsync(room, cancellationToken);
        return ServiceResult.Created(room);
    }

    public async Task<Result<RoomItem>> UpdateAsync(Guid roomId, RoomUpdateRequest? request, CancellationToken cancellationToken = default)
    {
        if (request == null)
        {
            return Result.Failure<RoomItem>($"Room update request should not be null [{nameof(request)}]");
        }

        var name = request.Name?.Trim();
        if (string.IsNullOrEmpty(name))
        {
            return Result.Failure<RoomItem>("Room name should not be empty");
        }

        var twitchChannel = request.TwitchChannel?.Trim();
        if (string.IsNullOrEmpty(twitchChannel))
        {
            return Result.Failure<RoomItem>("Room twitch channel should not be empty");
        }

        var foundRoom = await _roomRepository.FindByIdAsync((Guid)roomId, cancellationToken);
        if (foundRoom == null)
        {
            return Result.Failure<RoomItem>($"Not found room with id [{roomId}]");
        }

        foundRoom.Name = name;
        foundRoom.TwitchChannel = twitchChannel;
        await _roomRepository.UpdateAsync(foundRoom, cancellationToken);
        return new RoomItem { Id = foundRoom.Id, Name = foundRoom.Name };
    }

    public async Task<Result<Room?>> AddParticipantAsync(Guid roomId, Guid userId, CancellationToken cancellationToken = default)
    {
        var currentRoom = await _roomRepository.FindByIdAsync(roomId, cancellationToken);
        if (currentRoom == null)
        {
            return Result.Failure<Room?>($"Room not found by id {roomId}");
        }

        var user = await _userRepository.FindByIdAsync(userId, cancellationToken);
        if (user == null)
        {
            return Result.Failure<Room?>($"User not found by id {userId}");
        }

        if (await _roomRepository.HasUserAsync(roomId, user.Id, cancellationToken))
        {
            return currentRoom;
        }

        var participant = new RoomParticipant(user, currentRoom, RoomParticipantType.Viewer);
        currentRoom.Participants.Add(participant);
        await _roomRepository.UpdateAsync(currentRoom, cancellationToken);
        return currentRoom;
    }

    public async Task<Result<ServiceResult, ServiceError>> SendEventRequestAsync(IEventRequest request, CancellationToken cancellationToken = default)
    {
        var eventSpecification = new Spec<AppEvent>(e => e.Type == request.Type);
        var dbEvent = await _eventRepository.FindFirstOrDefaultAsync(eventSpecification, cancellationToken);
        if (dbEvent is null)
        {
            return ServiceError.NotFound($"Event not found by type {request.Type}");
        }

        var currentRoom = await _roomRepository.FindByIdAsync(request.RoomId, cancellationToken);
        if (currentRoom == null)
        {
            return ServiceError.NotFound($"Room not found by id {request.RoomId}");
        }

        var user = await _userRepository.FindByIdDetailedAsync(request.UserId, cancellationToken);
        if (user == null)
        {
            return ServiceError.NotFound($"User not found by id {request.UserId}");
        }

        var userRoles = user.Roles.Select(e => e.Id).ToHashSet();
        if (dbEvent.Roles.Any(e => !userRoles.Contains(e.Id)))
        {
            return ServiceError.AccessDenied();
        }

        await _roomEventDispatcher.WriteAsync(request.ToRoomEvent(), cancellationToken);
        return ServiceResult.Ok();
    }

    /// <summary>
    /// Close non closed room.
    /// </summary>
    /// <param name="roomId">Room id.</param>
    /// <param name="cancellationToken">Token.</param>
    /// <returns>Result.</returns>
    public async Task<Result<ServiceResult, ServiceError>> CloseRoomAsync(Guid roomId, CancellationToken cancellationToken = default)
    {
        var currentRoom = await _roomRepository.FindByIdAsync(roomId, cancellationToken);
        if (currentRoom == null)
        {
            return ServiceError.NotFound($"Room not found by id {roomId}");
        }

        if (currentRoom.Status == SERoomStatus.Close)
        {
            return ServiceError.Error("Room already closed");
        }

        await _roomQuestionRepository.CloseActiveQuestionAsync(roomId, cancellationToken);
        currentRoom.Status = SERoomStatus.Close;
        await _roomRepository.UpdateAsync(currentRoom, cancellationToken);
        return ServiceResult.Ok();
    }

    public async Task<Result<ServiceResult, ServiceError>> StartReviewRoomAsync(Guid roomId, CancellationToken cancellationToken)
    {
        var currentRoom = await _roomRepository.FindByIdAsync(roomId, cancellationToken);
        if (currentRoom == null)
        {
            return ServiceError.NotFound($"Room not found by id {roomId}");
        }

        if (currentRoom.Status == SERoomStatus.Review)
        {
            return ServiceError.Error("Room already reviewed");
        }

        currentRoom.Status = SERoomStatus.Review;
        await _roomRepository.UpdateAsync(currentRoom, cancellationToken);
        return ServiceResult.Ok();
    }

    public async Task<Result<ServiceResult<RoomState>, ServiceError>> GetRoomStateAsync(Guid roomId, CancellationToken cancellationToken = default)
    {
        var roomState = await _roomRepository.FindByIdDetailedAsync(roomId, RoomState.Mapper, cancellationToken);
        if (roomState == null)
        {
            return ServiceError.NotFound($"Not found room by id {roomId}");
        }

        var spec = new RoomReactionsSpecification(roomId);
        var reactions = await _roomQuestionReactionRepository.FindDetailed(spec, ReactionTypeOnlyMapper.Instance, cancellationToken);
        roomState.DislikeCount = reactions.Count(e => e == ReactionType.Dislike);
        roomState.LikeCount = reactions.Count(e => e == ReactionType.Like);
        return ServiceResult.Ok(roomState);
    }

    public async Task<Result<ServiceResult<Analytics>, ServiceError>> GetAnalyticsAsync(RoomAnalyticsRequest request, CancellationToken cancellationToken = default)
    {
        var analytics = await _roomRepository.GetAnalyticsAsync(request, cancellationToken);
        if (analytics == null)
        {
            return ServiceError.NotFound($"Room not found by id {request.RoomId}");
        }

        return ServiceResult.Ok(analytics);
    }

    public async Task<Result<ServiceResult<AnalyticsSummary>, ServiceError>> GetAnalyticsSummaryAsync(RoomAnalyticsRequest request, CancellationToken cancellationToken = default)
    {
        var analytics = await _roomRepository.GetAnalyticsSummaryAsync(request, cancellationToken);
        if (analytics == null)
        {
            return ServiceError.NotFound($"Room not found by id {request.RoomId}");
        }

        return ServiceResult.Ok(analytics);
    }

    private string FindNotFoundEntityIds<T>(IEnumerable<Guid> guids, IEnumerable<T> collection)
        where T : Entity
    {
        var notFoundEntityIds = guids.Except(collection.Select(entity => entity.Id));

        return string.Join(", ", notFoundEntityIds);
    }

    private async Task<Result<List<T>, ServiceError>> FindByIdsOrErrorAsync<T>(
        IRepository<T> repository,
        ICollection<Guid> ids,
        string entityName,
        CancellationToken cancellationToken)
        where T : Entity
    {
        var entities = await repository.FindByIdsAsync(ids, cancellationToken);
        var notFoundEntities = FindNotFoundEntityIds(ids, entities);
        if (!string.IsNullOrEmpty(notFoundEntities))
        {
            return ServiceError.Error($"Not found {entityName} with id [{notFoundEntities}]");
        }

        return entities;
    }
}
