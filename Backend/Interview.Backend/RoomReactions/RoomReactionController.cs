using Interview.Backend.Auth;
using Interview.Backend.Responses;
using Interview.Domain.RoomQuestionReactions;
using Interview.Domain.RoomQuestionReactions.Records;
using Interview.Domain.RoomQuestionReactions.Records.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Interview.Backend.RoomReactions;

[ApiController]
[Route("room-reactions")]
public class RoomReactionController : ControllerBase
{
    private readonly RoomQuestionReactionService _roomQuestionReactionService;

    public RoomReactionController(RoomQuestionReactionService roomQuestionReactionService)
    {
        _roomQuestionReactionService = roomQuestionReactionService;
    }

    /// <summary>
    ///  Creating a reaction on a question and a room.
    /// </summary>
    /// <param name="request">The user request.</param>
    /// <returns>Data about the new bundle (reaction, room, question).</returns>
    [Authorize]
    [HttpPost]
    [ProducesResponseType(typeof(RoomQuestionReactionDetail), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(MessageResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(MessageResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(MessageResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(MessageResponse), StatusCodes.Status500InternalServerError)]
    public Task<ActionResult<RoomQuestionReactionDetail>> CreateInRoom(
        [FromBody] RoomQuestionReactionCreateRequest request)
    {
        var user = User.ToUser();

        if (user == null)
        {
            return Task.FromResult<ActionResult<RoomQuestionReactionDetail>>(Unauthorized());
        }

        return _roomQuestionReactionService.CreateInRoomAsync(request, user.Id).ToResponseAsync();
    }

    /// <summary>
    ///  Send new event for question and a room.
    /// </summary>
    /// <param name="request">The user request.</param>
    /// <returns>Data about the new bundle (reaction, room, question).</returns>
    [Authorize(policy: GulagSecurePolicy.Manager)]
    [HttpPost("event")]
    [ProducesResponseType(typeof(RoomQuestionReaction), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(MessageResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(MessageResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(MessageResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(MessageResponse), StatusCodes.Status500InternalServerError)]
    public Task<ActionResult<RoomQuestionReaction>> SendReaction([FromBody] RoomQuestionSendReactionApiRequest request)
    {
        var user = User.ToUser();

        if (user == null)
        {
            return Task.FromResult<ActionResult<RoomQuestionReaction>>(Unauthorized());
        }

        var sendRequest = request.ToDomainRequest(user.Id);

        return _roomQuestionReactionService.SendReactionAsync(sendRequest, HttpContext.RequestAborted)
            .ToResponseAsync();
    }
}
