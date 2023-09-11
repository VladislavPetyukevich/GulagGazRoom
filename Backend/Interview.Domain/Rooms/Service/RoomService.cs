using Interview.Domain.Events;
using Interview.Domain.Questions;
using Interview.Domain.Reactions;
using Interview.Domain.Repository;
using Interview.Domain.RoomParticipants;
using Interview.Domain.RoomQuestionReactions;
using Interview.Domain.RoomQuestionReactions.Mappers;
using Interview.Domain.RoomQuestionReactions.Specifications;
using Interview.Domain.RoomQuestions;
using Interview.Domain.Rooms.Records.Request;
using Interview.Domain.Rooms.Records.Response.RoomStates;
using Interview.Domain.Rooms.Service.Records.Response;
using Interview.Domain.Rooms.Service.Records.Response.Detail;
using Interview.Domain.Rooms.Service.Records.Response.Page;
using Interview.Domain.Tags;
using Interview.Domain.Tags.Records.Response;
using Interview.Domain.Users;
using X.PagedList;
using Entity = Interview.Domain.Repository.Entity;

namespace Interview.Domain.Rooms.Service;

public sealed class RoomService : IRoomService
{
    private readonly IRoomRepository _roomRepository;
    private readonly IRoomQuestionRepository _roomQuestionRepository;
    private readonly IQuestionRepository _questionRepository;
    private readonly IUserRepository _userRepository;
    private readonly IRoomEventDispatcher _roomEventDispatcher;
    private readonly IRoomQuestionReactionRepository _roomQuestionReactionRepository;
    private readonly ITagRepository _tagRepository;

    public RoomService(
        IRoomRepository roomRepository,
        IRoomQuestionRepository roomQuestionRepository,
        IQuestionRepository questionRepository,
        IUserRepository userRepository,
        IRoomEventDispatcher roomEventDispatcher,
        IRoomQuestionReactionRepository roomQuestionReactionRepository,
        ITagRepository tagRepository)
    {
        _roomRepository = roomRepository;
        _questionRepository = questionRepository;
        _userRepository = userRepository;
        _roomEventDispatcher = roomEventDispatcher;
        _roomQuestionReactionRepository = roomQuestionReactionRepository;
        _tagRepository = tagRepository;
        _roomQuestionRepository = roomQuestionRepository;
    }

    public Task<IPagedList<RoomPageDetail>> FindPageAsync(
        int pageNumber, int pageSize, CancellationToken cancellationToken = default)
    {
        return _roomRepository.GetDetailedPageAsync(pageNumber, pageSize, cancellationToken);
    }

    public async Task<RoomDetail> FindByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var room = await _roomRepository.GetByIdAsync(id, cancellationToken);

        if (room is null)
        {
            throw NotFoundException.Create<Room>(id);
        }

        return room;
    }

    public async Task<Room> CreateAsync(
        RoomCreateRequest request, CancellationToken cancellationToken = default)
    {
        if (request is null)
        {
            throw new UserException(nameof(request));
        }

        var name = request.Name.Trim();

        if (string.IsNullOrEmpty(name))
        {
            throw new UserException("Room name should not be empty");
        }

        var questions =
            await FindByIdsOrErrorAsync(_questionRepository, request.Questions, "questions", cancellationToken);

        var experts = await FindByIdsOrErrorAsync(_userRepository, request.Experts, "experts", cancellationToken);

        var examinees = await FindByIdsOrErrorAsync(_userRepository, request.Examinees, "examinees", cancellationToken);

        var tags = await Tag.EnsureValidTagsAsync(_tagRepository, request.Tags, cancellationToken);

        var twitchChannel = request.TwitchChannel?.Trim();
        if (string.IsNullOrEmpty(twitchChannel))
        {
            throw new UserException("Twitch channel should not be empty");
        }

        var room = new Room(name, twitchChannel)
        {
            Tags = tags,
        };
        var roomQuestions = questions.Select(question =>
            new RoomQuestion { Room = room, Question = question, State = RoomQuestionState.Open });

        room.Questions.AddRange(roomQuestions);

        var participantsExperts = experts
            .Select(user => new RoomParticipant(user, room, RoomParticipantType.Expert))
            .ToList();

        var participantsExaminees = examinees
            .Select(user => new RoomParticipant(user, room, RoomParticipantType.Examinee))
            .ToList();

        room.Participants.AddRange(participantsExperts);
        room.Participants.AddRange(participantsExaminees);

        await _roomRepository.CreateAsync(room, cancellationToken);

        return room;
    }

    public async Task<RoomItem> UpdateAsync(
        Guid roomId, RoomUpdateRequest? request, CancellationToken cancellationToken = default)
    {
        if (request is null)
        {
            throw new UserException($"Room update request should not be null [{nameof(request)}]");
        }

        var name = request.Name?.Trim();
        if (string.IsNullOrEmpty(name))
        {
            throw new UserException("Room name should not be empty");
        }

        var twitchChannel = request.TwitchChannel?.Trim();
        if (string.IsNullOrEmpty(twitchChannel))
        {
            throw new UserException("Room twitch channel should not be empty");
        }

        var foundRoom = await _roomRepository.FindByIdAsync(roomId, cancellationToken);
        if (foundRoom is null)
        {
            throw NotFoundException.Create<User>(roomId);
        }

        var tags = await Tag.EnsureValidTagsAsync(_tagRepository, request.Tags, cancellationToken);

        foundRoom.Name = name;
        foundRoom.TwitchChannel = twitchChannel;

        foundRoom.Tags.Clear();
        foundRoom.Tags.AddRange(tags);
        await _roomRepository.UpdateAsync(foundRoom, cancellationToken);

        return new RoomItem
        {
            Id = foundRoom.Id,
            Name = foundRoom.Name,
            Tags = tags.Select(t => new TagItem
            {
                Id = t.Id,
                Value = t.Value,
                HexValue = t.HexColor,
            }).ToList(),
        };
    }

    public async Task<Room> AddParticipantAsync(
        Guid roomId, Guid userId, CancellationToken cancellationToken = default)
    {
        var currentRoom = await _roomRepository.FindByIdAsync(roomId, cancellationToken);
        if (currentRoom is null)
        {
            throw NotFoundException.Create<Room>(roomId);
        }

        var user = await _userRepository.FindByIdAsync(userId, cancellationToken);
        if (user is null)
        {
            throw NotFoundException.Create<User>(userId);
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

    public async Task SendEventRequestAsync(
        IEventRequest request, CancellationToken cancellationToken = default)
    {
        var currentRoom = await _roomRepository.FindByIdAsync(request.RoomId, cancellationToken);
        if (currentRoom is null)
        {
            throw NotFoundException.Create<Room>(request.RoomId);
        }

        var user = await _userRepository.FindByIdAsync(request.UserId, cancellationToken);
        if (user is null)
        {
            throw NotFoundException.Create<User>(request.UserId);
        }

        await _roomEventDispatcher.WriteAsync(request.ToRoomEvent(), cancellationToken);
    }

    /// <summary>
    /// Close non closed room.
    /// </summary>
    /// <param name="roomId">Room id.</param>
    /// <param name="cancellationToken">Token.</param>
    /// <returns>Result.</returns>
    public async Task CloseAsync(
        Guid roomId,
        CancellationToken cancellationToken = default)
    {
        var currentRoom = await _roomRepository.FindByIdAsync(roomId, cancellationToken);
        if (currentRoom == null)
        {
            throw NotFoundException.Create<Room>(roomId);
        }

        if (currentRoom.Status == SERoomStatus.Close)
        {
            throw new UserException("Room already closed");
        }

        await _roomQuestionRepository.CloseActiveQuestionAsync(roomId, cancellationToken);

        currentRoom.Status = SERoomStatus.Close;

        await _roomRepository.UpdateAsync(currentRoom, cancellationToken);
    }

    public async Task StartReviewAsync(Guid roomId, CancellationToken cancellationToken)
    {
        var currentRoom = await _roomRepository.FindByIdAsync(roomId, cancellationToken);
        if (currentRoom is null)
        {
            throw NotFoundException.Create<Room>(roomId);
        }

        if (currentRoom.Status == SERoomStatus.Review)
        {
            throw new UserException("Room already reviewed");
        }

        currentRoom.Status = SERoomStatus.Review;

        await _roomRepository.UpdateAsync(currentRoom, cancellationToken);
    }

    public async Task<RoomState> GetStateAsync(
        Guid roomId,
        CancellationToken cancellationToken = default)
    {
        var roomState = await _roomRepository.FindByIdDetailedAsync(roomId, RoomState.Mapper, cancellationToken);

        if (roomState == null)
        {
            throw NotFoundException.Create<Room>(roomId);
        }

        var spec = new RoomReactionsSpecification(roomId);

        var reactions = await _roomQuestionReactionRepository.FindDetailed(
            spec,
            ReactionTypeOnlyMapper.Instance,
            cancellationToken);

        roomState.DislikeCount = reactions.Count(e => e == ReactionType.Dislike);
        roomState.LikeCount = reactions.Count(e => e == ReactionType.Like);

        return roomState;
    }

    public async Task<Analytics> GetAnalyticsAsync(
        RoomAnalyticsRequest request,
        CancellationToken cancellationToken = default)
    {
        var analytics = await _roomRepository.GetAnalyticsAsync(request, cancellationToken);

        if (analytics is null)
        {
            throw NotFoundException.Create<Room>(request.RoomId);
        }

        return analytics;
    }

    public async Task<AnalyticsSummary> GetAnalyticsSummaryAsync(
        RoomAnalyticsRequest request, CancellationToken cancellationToken = default)
    {
        var analytics = await _roomRepository.GetAnalyticsSummaryAsync(request, cancellationToken);

        if (analytics is null)
        {
            throw NotFoundException.Create<Room>(request.RoomId);
        }

        return analytics;
    }

    private string FindNotFoundEntityIds<T>(IEnumerable<Guid> guids, IEnumerable<T> collection)
        where T : Entity
    {
        var notFoundEntityIds = guids.Except(collection.Select(entity => entity.Id));

        return string.Join(", ", notFoundEntityIds);
    }

    private async Task<List<T>> FindByIdsOrErrorAsync<T>(
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
            throw new NotFoundException($"Not found {entityName} with id [{notFoundEntities}]");
        }

        return entities;
    }
}
