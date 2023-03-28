using CSharpFunctionalExtensions;
using Interview.Domain.Questions;
using Interview.Domain.Reactions;
using Interview.Domain.Repository;
using Interview.Domain.RoomQuestionReactions.Mappers;
using Interview.Domain.RoomQuestionReactions.Specifications;
using Interview.Domain.RoomQuestions.Records;
using Interview.Domain.RoomQuestions.Records.Response;
using Interview.Domain.Rooms;
using NSpecifications;

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
            RoomQuestionChangeActiveRequest request, CancellationToken cancellationToken = default)
        {
            var roomQuestion = await _roomQuestionRepository.FindFirstByQuestionIdAndRoomIdAsync(
                request.QuestionId, request.RoomId, cancellationToken);

            if (roomQuestion == null)
            {
                return Result.Failure<RoomQuestionDetail?>($"Question in room not found by id {request.QuestionId}");
            }

            if (roomQuestion.State == RoomQuestionState.Active)
            {
                return Result.Failure<RoomQuestionDetail?>($"Question already has active state");
            }

            var roomQuestionActual = await _roomQuestionRepository.FindFirstByRoomAndStateAsync(
                request.RoomId,
                RoomQuestionState.Active,
                cancellationToken);

            if (roomQuestionActual != null)
            {
                roomQuestionActual.State = RoomQuestionState.Closed;

                await _roomQuestionRepository.UpdateAsync(roomQuestionActual, cancellationToken);
            }

            roomQuestion.State = RoomQuestionState.Active;

            await _roomQuestionRepository.UpdateAsync(roomQuestion, cancellationToken);

            return new RoomQuestionDetail
            {
                Id = roomQuestion.Id,
                RoomId = roomQuestion.Room.Id,
                QuestionId = roomQuestion.Question.Id,
                State = roomQuestion.State,
            };
        }

        /// <summary>
        /// Adding a question to a room.
        /// </summary>
        /// <param name="request">Request with data to add a question to the room.</param>
        /// <param name="cancellationToken">Task cancellation token.</param>
        /// <returns>The data of the new entry about the participant of the room.</returns>
        public async Task<Result<RoomQuestionDetail?>> CreateAsync(
            RoomQuestionCreateRequest request,
            CancellationToken cancellationToken)
        {
            var roomQuestion = await _roomQuestionRepository.FindFirstByQuestionIdAndRoomIdAsync(
                request.QuestionId, request.RoomId, cancellationToken);

            if (roomQuestion != null)
            {
                return Result.Failure<RoomQuestionDetail?>(
                    $"The room {request.RoomId} with question {request.QuestionId} already exists");
            }

            var room = await _roomRepository.FindByIdAsync(request.RoomId, cancellationToken);

            if (room == null)
            {
                return Result.Failure<RoomQuestionDetail?>($"Room with id {request.RoomId} not found");
            }

            var question = await _questionRepository.FindByIdAsync(request.QuestionId, cancellationToken);

            if (question == null)
            {
                return Result.Failure<RoomQuestionDetail?>($"Question with id {request.QuestionId} not found");
            }

            var newRoomQuestion = new RoomQuestion { Room = room, Question = question, State = RoomQuestionState.Open };

            await _roomQuestionRepository.CreateAsync(newRoomQuestion, cancellationToken);

            return new RoomQuestionDetail
            {
                Id = newRoomQuestion.Id,
                QuestionId = question.Id,
                RoomId = room.Id,
                State = newRoomQuestion.State,
            };
        }

        public async Task<Result<List<Guid>>> GetRoomQuestionsAsync(RoomQuestionsRequest request, CancellationToken cancellationToken = default)
        {
            var hasRoom = await _roomRepository.HasAsync(new Spec<Room>(room => room.Id == request.RoomId), cancellationToken);
            if (!hasRoom)
            {
                return Result.Failure<List<Guid>>($"Room not found by id {request.RoomId}");
            }

            var state = RoomQuestionState.FromValue((int)request.State);
            var specification = new Spec<RoomQuestion>(rq => rq.Room.Id == request.RoomId && rq.State == state);
            var mapper = new Mapper<RoomQuestion, Guid>(rq => rq.Question.Id);
            return await _roomQuestionRepository.FindAsync(specification, mapper, cancellationToken);
        }
    }
}
