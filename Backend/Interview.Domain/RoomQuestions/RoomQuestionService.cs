using CSharpFunctionalExtensions;
using Interview.Domain.RoomQuestions.Records;
using Interview.Domain.RoomQuestions.Records.Response;

namespace Interview.Domain.RoomQuestions;

public class RoomQuestionService
{
    private readonly IRoomQuestionRepository _roomQuestionRepository;

    public RoomQuestionService(IRoomQuestionRepository roomQuestionRepository)
    {
        _roomQuestionRepository = roomQuestionRepository;
    }

    public async Task<Result<RoomQuestionDetail?>> ChangeActiveQuestionAsync(
        RoomQuestionChangeActiveRequest request)
    {
        var roomQuestion = await _roomQuestionRepository.FindFirstByQuestionIdAndRoomIdAsync(request.QuestionId, request.RoomId, default);

        if (roomQuestion == null)
        {
            return Result.Failure<RoomQuestionDetail?>($"Question in room not found by id {request.QuestionId}");
        }

        if (roomQuestion.State == RoomQuestionState.Active)
        {
            return Result.Failure<RoomQuestionDetail?>($"Question already has active state");
        }

        var roomQuestionActual = await _roomQuestionRepository.FindFirstByRoomAndStateAsync(request.RoomId, RoomQuestionState.Active);

        if (roomQuestionActual != null)
        {
            roomQuestionActual.State = RoomQuestionState.Closed;

            await _roomQuestionRepository.UpdateAsync(roomQuestionActual);
        }

        roomQuestion.State = RoomQuestionState.Active;

        await _roomQuestionRepository.UpdateAsync(roomQuestion);

        return new RoomQuestionDetail
        {
            RoomId = roomQuestion.Room.Id,
            QuestionId = roomQuestion.Question.Id,
            State = roomQuestion.State,
        };
    }
}
