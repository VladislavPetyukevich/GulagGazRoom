using Interview.Backend.Auth;
using Interview.Domain.RoomQuestionReactions;
using Interview.Domain.RoomQuestionReactions.Records;
using Interview.Domain.RoomQuestionReactions.Records.Response;
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
            return BadRequest("User not found");
        }

        var createRoomQuestionReactionResult = await _roomQuestionReactionService.CreateInRoomAsync(request, user.Id);

        if (createRoomQuestionReactionResult.IsFailure)
        {
            return BadRequest($"Not found active question by room id {request.RoomId}");
        }

        return Ok(createRoomQuestionReactionResult.Value);
    }
}
