using CSharpFunctionalExtensions;
using Interview.Domain.Reactions;
using Interview.Domain.RoomQuestionReactions.Records;
using Interview.Domain.RoomQuestionReactions.Records.Response;
using Interview.Domain.RoomQuestions;
using Interview.Domain.Users;

namespace Interview.Domain.RoomQuestionReactions
{
    public class RoomQuestionReactionService
    {
        private readonly IRoomQuestionReactionRepository _roomQuestionReactionRepository;
        private readonly IRoomQuestionRepository _questionRepository;
        private readonly IReactionRepository _reactionRepository;
        private readonly IUserRepository _userRepository;

        public RoomQuestionReactionService(
            IRoomQuestionReactionRepository roomQuestionReactionRepository,
            IRoomQuestionRepository questionRepository,
            IReactionRepository reactionRepository,
            IUserRepository userRepository)
        {
            _roomQuestionReactionRepository = roomQuestionReactionRepository;
            _questionRepository = questionRepository;
            _reactionRepository = reactionRepository;
            _userRepository = userRepository;
        }

        public async Task<Result<RoomQuestionReactionDetail?>> CreateInRoomAsync(
            RoomQuestionReactionCreateRequest request,
            Guid userId)
        {
            var user = await _userRepository.FindByIdAsync(userId);

            if (user == null)
            {
                return Result.Failure<RoomQuestionReactionDetail?>(
                    $"User not found by user id {userId}");
            }

            var roomQuestion =
                await _questionRepository.FindFirstByRoomAndStateAsync(request.RoomId, RoomQuestionState.Active);

            if (roomQuestion == null)
            {
                return Result.Failure<RoomQuestionReactionDetail?>(
                    $"Active question not found in room id {request.RoomId}");
            }

            var reaction = await _reactionRepository.FindByIdAsync(request.ReactionId);

            if (reaction == null)
            {
                return Result.Failure<RoomQuestionReactionDetail?>($"Reaction not found by id {request.ReactionId}");
            }

            var questionReaction =
                new RoomQuestionReaction { RoomQuestion = roomQuestion, Reaction = reaction, Sender = user };

            await _roomQuestionReactionRepository.CreateAsync(questionReaction);

            return new RoomQuestionReactionDetail
            {
                RoomId = request.RoomId,
                Question = roomQuestion.Question.Id,
                Reaction = reaction.Id,
            };
        }
    }
}
