using Interview.Backend.Auth;
using Interview.Backend.Responses;
using Interview.Domain.RoomParticipants.Records.Request;
using Interview.Domain.RoomParticipants.Records.Response;
using Interview.Domain.RoomParticipants.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Interview.Backend.RoomParticipants;

[ApiController]
[Route("room-participants")]
public class RoomParticipantController : ControllerBase
{
    private readonly RoomParticipantService _roomParticipantService;

    public RoomParticipantController(RoomParticipantService roomParticipantService)
    {
        _roomParticipantService = roomParticipantService;
    }

    /// <summary>
    /// Getting a list of room participants.
    /// </summary>
    /// <param name="request">Page Parameters.</param>
    /// <returns>List of room participants.</returns>
    [Authorize(policy: GulagSecurePolicy.User)]
    [HttpGet]
    [ProducesResponseType(typeof(RoomParticipantDetail), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(MessageResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(MessageResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(MessageResponse), StatusCodes.Status500InternalServerError)]
    public Task<ActionResult<RoomParticipantDetail>> GetParticipant([FromQuery] RoomParticipantGetRequest request)
    {
        return _roomParticipantService.GetParticipantAsync(request, HttpContext.RequestAborted).ToResponseAsync();
    }

    /// <summary>
    /// Adding a transaction participant to a room.
    /// </summary>
    /// <param name="request">Data for adding a participant to a room.</param>
    /// <returns>Data about the added participant to the room.</returns>
    [Authorize(policy: GulagSecurePolicy.Manager)]
    [HttpPost]
    [ProducesResponseType(typeof(RoomParticipantDetail), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(MessageResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(MessageResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(MessageResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(MessageResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(MessageResponse), StatusCodes.Status500InternalServerError)]
    public Task<ActionResult<RoomParticipantDetail>> CreateParticipant([FromBody] RoomParticipantCreateRequest request)
    {
        return _roomParticipantService.CreateParticipantAsync(request, HttpContext.RequestAborted).ToResponseAsync();
    }

    /// <summary>
    /// Changing the status of a room participant.
    /// </summary>
    /// <param name="request">Data changes in the status of a room participant.</param>
    /// <returns>Information about the participant of the room.</returns>
    [Authorize(policy: GulagSecurePolicy.Manager)]
    [HttpPatch]
    [ProducesResponseType(typeof(RoomParticipantDetail), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(MessageResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(MessageResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(MessageResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(MessageResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(MessageResponse), StatusCodes.Status500InternalServerError)]
    public Task<ActionResult<RoomParticipantDetail>> ChangeParticipantStatus(
        [FromBody] RoomParticipantChangeStatusRequest request)
    {
        return _roomParticipantService.ChangeParticipantStatusAsync(request, HttpContext.RequestAborted)
            .ToResponseAsync();
    }
}
