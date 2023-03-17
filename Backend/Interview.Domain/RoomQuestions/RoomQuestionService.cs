using CSharpFunctionalExtensions;
using Interview.Domain.Questions;
using Interview.Domain.RoomQuestions.Records;
using Interview.Domain.RoomQuestions.Records.Response;
using Interview.Domain.RoomQuestions.Records.Response.Response;
using Interview.Domain.Rooms;

namespace Interview.Domain.RoomQuestions
{
    public class RoomQuestionService
    {
        private readonly IRoomQuestionRepository _roomQuestionRepository;

        private readonly IQuestionRepository _questionRepository;

        private readonly IRoomRepository _roomRepository;

        public RoomQuestionService(
            IRoomQuestionRepository roomQuestionRepository,
            IRoomRepository roomRepository,
            IQuestionRepository questionRepository)
        {
            _roomQuestionRepository = roomQuestionRepository;
            _roomRepository = roomRepository;
            _questionRepository = questionRepository;
        }

        public async Task<Result<RoomQuestionDetail?>> ChangeActiveQuestionAsync(
            RoomQuestionChangeActiveRequest request)
        {
            var roomQuestion =
                await _roomQuestionRepository.FindFirstByQuestionIdAndRoomIdAsync(request.QuestionId, request.RoomId,
                    default);

            if (roomQuestion == null)
            {
                return Result.Failure<RoomQuestionDetail?>($"Question in room not found by id {request.QuestionId}");
            }

            if (roomQuestion.State == RoomQuestionState.Active)
            {
                return Result.Failure<RoomQuestionDetail?>($"Question already has active state");
            }

            var roomQuestionActual =
                await _roomQuestionRepository.FindFirstByRoomAndStateAsync(request.RoomId, RoomQuestionState.Active);

            if (roomQuestionActual != null)
            {
                roomQuestionActual.State = RoomQuestionState.Closed;

                await _roomQuestionRepository.UpdateAsync(roomQuestionActual);
            }

            roomQuestion.State = RoomQuestionState.Active;

            await _roomQuestionRepository.UpdateAsync(roomQuestion);

            return new RoomQuestionDetail
            {
                RoomId = roomQuestion.Room.Id, QuestionId = roomQuestion.Question.Id, State = roomQuestion.State,
            };
        }

        public async Task<Result<RoomQuestionDetail?>> Create(RoomQuestionCreateRequest request)
        {
            var roomQuestion = await _roomQuestionRepository.FindFirstByQuestionIdAndRoomIdAsync(
                request.QuestionId, request.RoomId, default);

            if (roomQuestion != null)
            {
                return Result.Failure<RoomQuestionDetail?>(
                    $"The room {request.RoomId} with question {request.QuestionId} already exists");
            }

            var room = await _roomRepository.FindByIdAsync(request.RoomId);

            if (room == null)
            {
                return Result.Failure<RoomQuestionDetail?>($"Room with id {request.RoomId} not found");
            }

            var question = await _questionRepository.FindByIdAsync(request.QuestionId);

            if (question == null)
            {
                return Result.Failure<RoomQuestionDetail?>($"Question with id {request.QuestionId} not found");
            }

            var newRoomQuestion = new RoomQuestion { Room = room, Question = question, State = RoomQuestionState.Open };

            await _roomQuestionRepository.CreateAsync(newRoomQuestion);

            return new RoomQuestionDetail
            {
                Id = newRoomQuestion.Id, QuestionId = question.Id, RoomId = room.Id, State = newRoomQuestion.State,
            };
        }
    }
}
