using Interview.Backend.Auth;
using Interview.Backend.Responses;
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

    [Authorize]
    [HttpPost]
    [ProducesResponseType(typeof(RoomQuestionReactionDetail), 200)]
    [ProducesResponseType(typeof(string), 400)]
    public Task<ActionResult<RoomQuestionReactionDetail>> CreateInRoom([FromBody] RoomQuestionReactionCreateRequest request)
    {
        var user = HttpContext.User.ToUser();

        if (user == null)
        {
            return Task.FromResult<ActionResult<RoomQuestionReactionDetail>>(Unauthorized());
        }

        return _roomQuestionReactionService.CreateInRoomAsync(request, user.Id).ToResponseAsync();
    }

    [Authorize(policy: GulagSecurePolicy.Manager)]
    [HttpPost(nameof(SendReaction))]
    [ProducesResponseType(typeof(string), 200)]
    [ProducesResponseType(typeof(string), 400)]
    public Task<ActionResult<RoomQuestionReaction>> SendReaction(RoomQuestionSendReactionApiRequest request)
    {
        var user = User.ToUser();
        if (user == null)
        {
            return Task.FromResult<ActionResult<RoomQuestionReaction>>(Unauthorized());
        }

        var sendRequest = request.ToDomainRequest(user.Id);
        return _roomQuestionReactionService.SendReactionAsync(sendRequest, HttpContext.RequestAborted).ToResponseAsync();
    }
}
