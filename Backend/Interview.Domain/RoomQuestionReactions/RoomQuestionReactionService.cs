using CSharpFunctionalExtensions;
using Interview.Domain.Errors;
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

    public async Task<Result<ServiceResult<RoomQuestionReaction>, ServiceError>> SendReactionAsync(
        RoomQuestionSendReactionRequest request,
        CancellationToken cancellationToken = default)
    {
        var roomQuestion = await _questionRepository.FindFirstByQuestionIdAndRoomIdAsync(request.QuestionId, request.RoomId, default);

        if (roomQuestion == null)
        {
            return ServiceError.Error($"Question in room not found by id {request.QuestionId}");
        }

        var user = await _userRepository.FindByIdAsync(request.UserId, cancellationToken);
        if (user == null)
        {
            return ServiceError.Error($"Not found User by id {request.UserId}");
        }

        var reactionType = ReactionType.List.Single(e => e.EventType == request.Type);
        var reaction = await _reactionRepository.FindByReactionTypeAsync(reactionType, cancellationToken);

        if (reaction == null)
        {
            return ServiceError.Error($"Reaction not found by event type {request.Type}");
        }

        var entity = new RoomQuestionReaction { Reaction = reaction, Sender = user, RoomQuestion = roomQuestion, };
        await _roomQuestionReactionRepository.CreateAsync(entity, cancellationToken);
        return ServiceResult.Created(entity);
    }

    public async Task<Result<ServiceResult<RoomQuestionReactionDetail>, ServiceError>> CreateInRoomAsync(
        RoomQuestionReactionCreateRequest request,
        Guid userId)
    {
        var user = await _userRepository.FindByIdAsync(userId);

        if (user == null)
        {
            return ServiceError.Error($"User not found by user id {userId}");
        }

        var roomQuestion =
            await _questionRepository.FindFirstByRoomAndStateAsync(request.RoomId, RoomQuestionState.Active);

        if (roomQuestion == null)
        {
            return ServiceError.Error($"Active question not found in room id {request.RoomId}");
        }

        var reaction = await _reactionRepository.FindByIdAsync(request.ReactionId);

        if (reaction == null)
        {
            return ServiceError.Error($"Reaction not found by id {request.ReactionId}");
        }

        var questionReaction = new RoomQuestionReaction
        {
            RoomQuestion = roomQuestion,
            Reaction = reaction,
            Sender = user,
        };

        await _roomQuestionReactionRepository.CreateAsync(questionReaction);

        return ServiceResult.Created(new RoomQuestionReactionDetail
        {
            RoomId = request.RoomId,
            Question = roomQuestion.Question.Id,
            Reaction = reaction.Id,
        });
    }
}
