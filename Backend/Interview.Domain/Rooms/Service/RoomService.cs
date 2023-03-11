using CSharpFunctionalExtensions;
using Interview.Domain.Questions;
using Interview.Domain.Users;

namespace Interview.Domain.Rooms.Service
{
    public sealed class RoomService
    {
        private readonly IRoomRepository _roomRepository;
        private readonly IUserRepository _userRepository;
        private readonly IQuestionRepository _questionRepository;

        public RoomService(IRoomRepository roomRepository, IUserRepository userRepository, IQuestionRepository questionRepository)
        {
            _roomRepository = roomRepository;
            _userRepository = userRepository;
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
    }
}
