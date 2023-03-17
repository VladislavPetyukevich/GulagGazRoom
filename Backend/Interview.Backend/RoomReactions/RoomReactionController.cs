using Interview.Backend.Auth;
using Interview.Domain.RoomQuestionReactions;
using Interview.Domain.RoomQuestionReactions.Records;
using Interview.Domain.RoomQuestionReactions.Records.Response;
using Interview.Domain.RoomQuestions.Records;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Interview.Backend.RoomReactions;

[ApiController]
[Route("[controller]")]
public class RoomReactionController : ControllerBase
{
    private readonly RoomQuestionReactionService _roomQuestionReactionService;

    public RoomReactionController(RoomQuestionReactionService roomQuestionReactionService)
    {
        _roomQuestionReactionService = roomQuestionReactionService;
    }

    [Authorize(policy: GulagSecurePolicy.Manager)]
    [HttpPost]
    [ProducesResponseType(typeof(RoomQuestionReactionDetail), 200)]
    [ProducesResponseType(typeof(string), 400)]
    public async Task<ActionResult<RoomQuestionReactionDetail?>> CreateInRoom([FromBody] RoomQuestionReactionCreateRequest request)
    {
        var user = HttpContext.User.ToUser();

        if (user == null)
        {
            return NotFound("Authorized user not found");
        }

        var createRoomQuestionReactionResult = await _roomQuestionReactionService.CreateInRoomAsync(request, user.Id);

        if (createRoomQuestionReactionResult.IsFailure)
        {
            return BadRequest(createRoomQuestionReactionResult.Error);
        }

        return Ok(createRoomQuestionReactionResult.Value);
    }

    [Authorize(policy: GulagSecurePolicy.Manager)]
    [HttpPost(nameof(SendReaction))]
    [ProducesResponseType(typeof(string), 200)]
    [ProducesResponseType(typeof(string), 400)]
    public async Task<ActionResult<RoomQuestionChangeActiveRequest?>> SendReaction(
        RoomQuestionSendReactionApiRequest request)
    {
        var user = User.ToUser();
        if (user == null)
        {
            return Unauthorized();
        }

        var sendRequest = request.ToRoomQuestionSendReactionRequest(user.Id);
        var result = await _roomQuestionReactionService.SendReactionAsync(sendRequest, HttpContext.RequestAborted);
        if (result.IsFailure)
        {
            return BadRequest(result.Error);
        }

        return Ok();
    }
}
