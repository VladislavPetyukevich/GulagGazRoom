using CSharpFunctionalExtensions;
using Interview.Domain.Questions;
using Interview.Domain.Rooms.Service.Records.Request;
using Interview.Domain.Rooms.Service.Records.Response;
using Interview.Domain.RoomUsers;
using Interview.Domain.Users;
using Entity = Interview.Domain.Repository.Entity;

namespace Interview.Domain.Rooms.Service
{
    public sealed class RoomService
    {
        private readonly IRoomRepository _roomRepository;
        private readonly IQuestionRepository _questionRepository;
        private readonly IUserRepository _userRepository;

        public RoomService(IRoomRepository roomRepository, IQuestionRepository questionRepository,
            IUserRepository userRepository)
        {
            _roomRepository = roomRepository;
            _questionRepository = questionRepository;
            _userRepository = userRepository;
        }

        public async Task<Result<Room>> CreateAsync(RoomCreateRequest request,
            CancellationToken cancellationToken = default)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            var name = request.Name?.Trim();
            if (string.IsNullOrEmpty(name))
            {
                return Result.Failure<Room>("Room name should not be empty");
            }

            var questions = await _questionRepository.FindByIdsAsync(request.Questions, cancellationToken);

            var questionsNotFound = FindNotFoundEntityIds(request.Questions, questions);

            if (!string.IsNullOrEmpty(questionsNotFound))
            {
                return Result.Failure<Room>($"Not found questions with id [{questionsNotFound}]");
            }

            var users = await _userRepository.FindByIdsAsync(request.Users, cancellationToken);

            var usersNotFound = FindNotFoundEntityIds(request.Users, users);

            if (!string.IsNullOrEmpty(usersNotFound))
            {
                return Result.Failure<Room>($"Not found users with id [{usersNotFound}]");
            }

            var newRoom = new Room(name);
            newRoom.Questions.AddRange(questions);

            newRoom.Participants.AddRange(
                users.Select(user => new RoomParticipant(user, newRoom, RoomParticipantType.Viewer)));

            await _roomRepository.CreateAsync(newRoom, cancellationToken);

            return newRoom;
        }

        public async Task<Result<RoomItem>> PatchUpdate(Guid? id, RoomPatchUpdateRequest? request)
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

            var foundRoom = await _roomRepository.FindByIdAsync((Guid)id);

            if (foundRoom == null)
            {
                return Result.Failure<RoomItem>($"Not found room with id [{id}]");
            }

            foundRoom.Name = name;

            await _roomRepository.UpdateAsync(foundRoom);

            return new RoomItem { Id = foundRoom.Id, Name = foundRoom.Name };
        }

        private static string FindNotFoundEntityIds<T>(HashSet<Guid> guids, List<T> collection)
            where T : Entity
        {
            var notFoundEntityIds = guids.Except(collection.Select(entity => entity.Id));

            return string.Join(", ", notFoundEntityIds);
        }
    }
}
