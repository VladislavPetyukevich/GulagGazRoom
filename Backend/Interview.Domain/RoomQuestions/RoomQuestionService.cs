using CSharpFunctionalExtensions;
using Interview.Domain.Questions;
using Interview.Domain.Reactions;
using Interview.Domain.Repository;
using Interview.Domain.RoomQuestionReactions.Mappers;
using Interview.Domain.RoomQuestionReactions.Specifications;
using Interview.Domain.RoomQuestions.Records;
using Interview.Domain.RoomQuestions.Records.Response;
using Interview.Domain.Rooms;
using Interview.Domain.ServiceResults;
using Interview.Domain.ServiceResults.Errors;
using Interview.Domain.ServiceResults.Success;
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

        public async Task<Result<ServiceResult<RoomQuestionDetail>, ServiceError>> ChangeActiveQuestionAsync(
            RoomQuestionChangeActiveRequest request, CancellationToken cancellationToken = default)
        {
            var roomQuestion = await _roomQuestionRepository.FindFirstByQuestionIdAndRoomIdAsync(
                request.QuestionId, request.RoomId, cancellationToken);

            if (roomQuestion == null)
            {
                return ServiceError.NotFound($"Question in room not found by id {request.QuestionId}");
            }

            if (roomQuestion.State == RoomQuestionState.Active)
            {
                return ServiceError.Error("Question already has active state");
            }

            var specification = new Spec<Room>(r => r.Id == request.RoomId && r.Status == RoomStatus.New);

            var room = await _roomRepository.FindFirstOrDefaultAsync(specification, cancellationToken);

            if (room != null)
            {
                room.Status = RoomStatus.Active;
                await _roomRepository.UpdateAsync(room, cancellationToken);
            }

            await _roomQuestionRepository.CloseActiveQuestionAsync(request.RoomId, cancellationToken);

            roomQuestion.State = RoomQuestionState.Active;

            await _roomQuestionRepository.UpdateAsync(roomQuestion, cancellationToken);

            return ServiceResult.Ok(new RoomQuestionDetail
            {
                Id = roomQuestion.Id,
                RoomId = roomQuestion.Room!.Id,
                QuestionId = roomQuestion.Question!.Id,
                State = roomQuestion.State,
            });
        }

        /// <summary>
        /// Adding a question to a room.
        /// </summary>
        /// <param name="request">Request with data to add a question to the room.</param>
        /// <param name="cancellationToken">Task cancellation token.</param>
        /// <returns>The data of the new entry about the participant of the room.</returns>
        public async Task<Result<ServiceResult<RoomQuestionDetail>, ServiceError>> CreateAsync(
            RoomQuestionCreateRequest request,
            CancellationToken cancellationToken)
        {
            var roomQuestion = await _roomQuestionRepository.FindFirstByQuestionIdAndRoomIdAsync(
                request.QuestionId, request.RoomId, cancellationToken);

            if (roomQuestion != null)
            {
                return ServiceError.Error($"The room {request.RoomId} with question {request.QuestionId} already exists");
            }

            var room = await _roomRepository.FindByIdAsync(request.RoomId, cancellationToken);

            if (room == null)
            {
                return ServiceError.NotFound($"Room with id {request.RoomId} not found");
            }

            var question = await _questionRepository.FindByIdAsync(request.QuestionId, cancellationToken);

            if (question == null)
            {
                return ServiceError.NotFound($"Question with id {request.QuestionId} not found");
            }

            var newRoomQuestion = new RoomQuestion { Room = room, Question = question, State = RoomQuestionState.Open };

            await _roomQuestionRepository.CreateAsync(newRoomQuestion, cancellationToken);

            return ServiceResult.Created(new RoomQuestionDetail
            {
                Id = newRoomQuestion.Id,
                QuestionId = question.Id,
                RoomId = room.Id,
                State = newRoomQuestion.State,
            });
        }

        public async Task<Result<ServiceResult<List<Guid>>, ServiceError>> FindRoomQuestionsAsync(RoomQuestionsRequest request, CancellationToken cancellationToken = default)
        {
            var hasRoom = await _roomRepository.HasAsync(new Spec<Room>(room => room.Id == request.RoomId), cancellationToken);
            if (!hasRoom)
            {
                return ServiceError.NotFound($"Room not found by id {request.RoomId}");
            }

            var state = RoomQuestionState.FromValue((int)request.State);
            var specification = new Spec<RoomQuestion>(rq => rq.Room.Id == request.RoomId && rq.State == state);
            var mapper = new Mapper<RoomQuestion, Guid>(rq => rq.Question.Id);
            var questions = await _roomQuestionRepository.FindAsync(specification, mapper, cancellationToken);
            return ServiceResult.Ok(questions);
        }
    }
}
