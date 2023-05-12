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
    ///  Creates a reaction from the user to an active question in the room.
    /// </summary>
    /// <param name="request">The user request.</param>
    /// <returns>Data about the new bundle (reaction, room, question).</returns>
    [Authorize]
    [HttpPost]
    [ProducesResponseType(typeof(RoomQuestionReactionDetail), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(MessageResponse), StatusCodes.Status404NotFound)]
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

        return _roomQuestionReactionService
            .CreateAsync(request, user.Id, HttpContext.RequestAborted)
            .ToResponseAsync();
    }
}
