using CSharpFunctionalExtensions;
using Interview.Domain.Questions;
using Interview.Domain.Rooms.Service.Records.Request;
using Interview.Domain.Rooms.Service.Records.Response;
using Interview.Domain.Rooms.Service.Records.Response.FindById;

namespace Interview.Domain.Rooms.Service
{
    public sealed class RoomService
    {
        private readonly IRoomRepository _roomRepository;
        private readonly IQuestionRepository _questionRepository;

        public RoomService(IRoomRepository roomRepository, IQuestionRepository questionRepository)
        {
            _roomRepository = roomRepository;
            _questionRepository = questionRepository;
        }

        public async Task<Result<Room>> CreateAsync(RoomCreateRequest request, CancellationToken cancellationToken = default)
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
            var notFoundQuestions = request.Questions.Except(questions.Select(e => e.Id));
            var notFoundQuestionsMessage = string.Join(", ", notFoundQuestions);
            if (!string.IsNullOrEmpty(notFoundQuestionsMessage))
            {
                return Result.Failure<Room>($"Not found questions with id [{notFoundQuestionsMessage}]");
            }

            var newRoom = new Room(name);
            newRoom.Questions.AddRange(questions);
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

        public async Task<Result<RoomFoundItem>> FindById(Guid? id, CancellationToken cancellationToken = default)
        {
            if (id == null)
            {
                return Result.Failure<RoomFoundItem>($"Room id should not be null [{nameof(id)}]");
            }

            var foundRoom = await _roomRepository.GetByIdAsync((Guid)id, cancellationToken);

            return foundRoom ?? Result.Failure<RoomFoundItem>($"Not found room with id [{id}]");
        }
    }
}
