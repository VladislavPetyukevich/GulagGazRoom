using CSharpFunctionalExtensions;
using Interview.Domain.Reactions;
using Interview.Domain.RoomQuestionReactions.Records;
using Interview.Domain.RoomQuestionReactions.Records.Response;
using Interview.Domain.RoomQuestions;
using Interview.Domain.ServiceResults;
using Interview.Domain.ServiceResults.Errors;
using Interview.Domain.ServiceResults.Success;
using Interview.Domain.Users;

namespace Interview.Domain.RoomQuestionReactions;

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

    /// <summary>
    /// Creates a reaction from the user to an active question in the room.
    /// </summary>
    /// <param name="request">Reaction data.</param>
    /// <param name="userId">User id.</param>
    /// <param name="cancellationToken">Token.</param>
    /// <returns>Result of operation.</returns>
    public async Task<Result<ServiceResult<RoomQuestionReactionDetail>, ServiceError>> CreateAsync(
        RoomQuestionReactionCreateRequest request,
        Guid userId,
        CancellationToken cancellationToken)
    {
        var user = await _userRepository.FindByIdAsync(userId, cancellationToken);

        if (user == null)
        {
            return ServiceError.NotFound($"User not found by user id {userId}");
        }

        var roomQuestion = await _questionRepository.FindFirstByRoomAndStateAsync(
            request.RoomId,
            RoomQuestionState.Active,
            cancellationToken);

        if (roomQuestion == null)
        {
            return ServiceError.NotFound($"Active question not found in room id {request.RoomId}");
        }

        var reaction = await _reactionRepository.FindByIdAsync(request.ReactionId, cancellationToken);

        if (reaction == null)
        {
            return ServiceError.NotFound($"Reaction not found by id {request.ReactionId}");
        }

        var questionReaction = new RoomQuestionReaction
        {
            RoomQuestion = roomQuestion,
            Reaction = reaction,
            Sender = user,
            Payload = request.Payload,
        };

        await _roomQuestionReactionRepository.CreateAsync(questionReaction, cancellationToken);

        return ServiceResult.Created(new RoomQuestionReactionDetail
        {
            RoomId = request.RoomId,
            Question = roomQuestion.Question!.Id,
            Reaction = reaction.Id,
        });
    }
}
