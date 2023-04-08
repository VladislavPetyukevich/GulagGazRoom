using CSharpFunctionalExtensions;
using Interview.Domain.Errors;
using Interview.Domain.Events;
using Interview.Domain.Events.Events;
using Interview.Domain.Questions;
using Interview.Domain.Reactions;
using Interview.Domain.RoomParticipants;
using Interview.Domain.RoomQuestionReactions;
using Interview.Domain.RoomQuestionReactions.Mappers;
using Interview.Domain.RoomQuestionReactions.Specifications;
using Interview.Domain.RoomQuestions;
using Interview.Domain.Rooms.Service.Records;
using Interview.Domain.Rooms.Service.Records.Request;
using Interview.Domain.Rooms.Service.Records.Response;
using Interview.Domain.Rooms.Service.Records.Response.RoomStates;
using Interview.Domain.ServiceResults;
using Interview.Domain.ServiceResults.Errors;
using Interview.Domain.ServiceResults.Success;
using Interview.Domain.Users;
using NSpecifications;
using Entity = Interview.Domain.Repository.Entity;

namespace Interview.Domain.Rooms.Service;

public sealed class RoomService
{
    private readonly IRoomRepository _roomRepository;
    private readonly IQuestionRepository _questionRepository;
    private readonly IUserRepository _userRepository;
    private readonly IRoomEventDispatcher _roomEventDispatcher;
    private readonly IRoomQuestionReactionRepository _roomQuestionReactionRepository;

    public RoomService(
        IRoomRepository roomRepository,
        IQuestionRepository questionRepository,
        IUserRepository userRepository,
        IRoomEventDispatcher roomEventDispatcher,
        IRoomQuestionReactionRepository roomQuestionReactionRepository)
    {
        _roomRepository = roomRepository;
        _questionRepository = questionRepository;
        _userRepository = userRepository;
        _roomEventDispatcher = roomEventDispatcher;
        _roomQuestionReactionRepository = roomQuestionReactionRepository;
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

        var questions = await _questionRepository.FindByIdsAsync(request.Questions, cancellationToken);

        var questionsNotFound = FindNotFoundEntityIds(request.Questions, questions);

        if (!string.IsNullOrEmpty(questionsNotFound))
        {
            return ServiceError.Error($"Not found questions with id [{questionsNotFound}]");
        }

        var users = await _userRepository.FindByIdsAsync(request.Users, cancellationToken);

        var usersNotFound = FindNotFoundEntityIds(request.Users, users);

        if (!string.IsNullOrEmpty(usersNotFound))
        {
            return ServiceError.Error($"Not found users with id [{usersNotFound}]");
        }

        var twitchChannel = request.TwitchChannel?.Trim();

        if (string.IsNullOrEmpty(twitchChannel))
        {
            return ServiceError.Error($"Twitch channel should not be empty");
        }

        var room = new Room(name, twitchChannel);

        var roomQuestions = questions.Select(question =>
            new RoomQuestion { Room = room, Question = question, State = RoomQuestionState.Open });

        room.Questions.AddRange(roomQuestions);

        var participants = users
            .Select(user => new RoomParticipant(user, room, RoomParticipantType.Viewer))
            .ToList();

        room.Participants.AddRange(participants);

        await _roomRepository.CreateAsync(room, cancellationToken);

        return ServiceResult.Created(room);
    }

    public async Task<Result<RoomItem>> PatchUpdate(Guid? id, RoomPatchUpdateRequest? request, CancellationToken cancellationToken = default)
    {
        if (id == null)
        {
            return Result.Failure<RoomItem>($"Room id should not be null [{nameof(id)}]");
        }

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

        var foundRoom = await _roomRepository.FindByIdAsync((Guid)id, cancellationToken);

        if (foundRoom == null)
        {
            return Result.Failure<RoomItem>($"Not found room with id [{id}]");
        }

        foundRoom.Name = name;
        foundRoom.TwitchChannel = twitchChannel;

        await _roomRepository.UpdateAsync(foundRoom, cancellationToken);

        return new RoomItem { Id = foundRoom.Id, Name = foundRoom.Name };
    }

    public async Task<Result<Room?>> PrepareRoomAsync(Guid roomIdentity, Guid userIdentity, CancellationToken cancellationToken = default)
    {
        var currentRoom = await _roomRepository.FindByIdAsync(roomIdentity, cancellationToken);
        if (currentRoom == null)
        {
            return Result.Failure<Room?>($"Room not found by id {roomIdentity}");
        }

        var user = await _userRepository.FindByIdAsync(userIdentity, cancellationToken);
        if (user == null)
        {
            return Result.Failure<Room?>($"User not found by id {userIdentity}");
        }

        if (await _roomRepository.HasUserAsync(roomIdentity, user.Id, cancellationToken))
        {
            return currentRoom;
        }

        var participant = new RoomParticipant(user, currentRoom, RoomParticipantType.Viewer);

        currentRoom.Participants.Add(participant);

        await _roomRepository.UpdateAsync(currentRoom, cancellationToken);

        return currentRoom;
    }

    public async Task<Result<ServiceResult, ServiceError>> SendGasEventAsync(SendGasRoomEventRequest request, CancellationToken cancellationToken = default)
    {
        var currentRoom = await _roomRepository.FindByIdAsync(request.RoomId, cancellationToken);
        if (currentRoom == null)
        {
            return ServiceError.NotFound($"Room not found by id {request.RoomId}");
        }

        var user = await _userRepository.FindByIdAsync(request.UserId, cancellationToken);
        if (user == null)
        {
            return ServiceError.NotFound($"User not found by id {request.UserId}");
        }

        await _roomEventDispatcher.WriteAsync(request.ToRoomEvent(), cancellationToken);
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

    private string FindNotFoundEntityIds<T>(IEnumerable<Guid> guids, IEnumerable<T> collection)
        where T : Entity
    {
        var notFoundEntityIds = guids.Except(collection.Select(entity => entity.Id));

        return string.Join(", ", notFoundEntityIds);
    }
}
