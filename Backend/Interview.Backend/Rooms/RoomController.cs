using Interview.Backend.Auth;
using Interview.Backend.Responses;
using Interview.Domain;
using Interview.Domain.Rooms.Service;
using Interview.Domain.Rooms.Service.Records.Request;
using Interview.Domain.Rooms.Service.Records.Response.Detail;
using Interview.Domain.Rooms.Service.Records.Response.Page;
using Interview.Domain.Rooms.Service.Records.Response.RoomStates;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using X.PagedList;

namespace Interview.Backend.Rooms;

[ApiController]
[Route("rooms")]
public class RoomController : ControllerBase
{
    private readonly IRoomRepository _roomRepository;
    private readonly RoomService _roomService;

    public RoomController(IRoomRepository userRepository, RoomService roomService)
    {
        _roomRepository = userRepository;
        _roomService = roomService;
    }

    /// <summary>
    /// Getting a Room page.
    /// </summary>
    /// <param name="request">Request.</param>
    /// <returns>Page.</returns>
    [Authorize]
    [HttpGet("")]
    public Task<IPagedList<RoomPageDetail>> GetPage([FromQuery] PageRequest request)
    {
        return _roomRepository.GetDetailedPageAsync(request.PageNumber, request.PageSize);
    }

    /// <summary>
    /// Getting a Room by ID.
    /// </summary>
    /// <param name="id">Id.</param>
    /// <returns>Room.</returns>
    [Authorize]
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(RoomDetail), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(MessageResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id)
    {
        var room = await _roomRepository.GetByIdAsync(id);
        if (room is null)
        {
            throw NotFoundException.Create<Room>(id);
        }

        return Ok(room);
    }

    /// <summary>
    /// Getting a Room state by id.
    /// </summary>
    /// <param name="id">Room id.</param>
    /// <returns>Room state.</returns>
    [Authorize]
    [HttpGet("{id:guid}/state")]
    [ProducesResponseType(typeof(RoomState), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(MessageResponse), StatusCodes.Status404NotFound)]
    public Task<ActionResult<RoomState>> GetRoomState(Guid id)
    {
        return _roomService.GetRoomStateAsync(id).ToResponseAsync();
    }

    /// <summary>
    /// Creating a new room.
    /// </summary>
    /// <param name="room">Room.</param>
    /// <returns>Created room.</returns>
    [Authorize(policy: GulagSecurePolicy.Manager)]
    [HttpPost("")]
    [ProducesResponseType(typeof(Guid), 201)]
    [ProducesResponseType(typeof(MessageResponse), StatusCodes.Status400BadRequest)]
    public Task<ActionResult<Room>> Create([FromBody] RoomCreateRequest room)
    {
        return _roomService.CreateAsync(room).ToResponseAsync();
    }

    /// <summary>
    /// Update room.
    /// </summary>
    /// <param name="id">Room id.</param>
    /// <param name="request">Request.</param>
    /// <returns>Ok message.</returns>
    [Authorize(policy: GulagSecurePolicy.Manager)]
    [HttpPatch("{id:guid}")]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(MessageResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> PatchUpdate(Guid id, [FromBody] RoomUpdateRequest request)
    {
        var updatedRoomResult = await _roomService.UpdateAsync(id, request);

        return updatedRoomResult.IsFailure
            ? BadRequest(updatedRoomResult.Error)
            : Ok(updatedRoomResult.Value);
    }

    /// <summary>
    /// Sending gas event to room.
    /// </summary>
    /// <param name="request">Request.</param>
    /// <returns>Ok message.</returns>
    [Authorize(policy: GulagSecurePolicy.Manager)]
    [HttpPost("event/gas")]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(MessageResponse), StatusCodes.Status400BadRequest)]
    public Task<ActionResult> SendGasEvent(SendGasRoomEventApiRequest request)
    {
        var user = User.ToUser();
        if (user == null)
        {
            return Task.FromResult<ActionResult>(Unauthorized());
        }

        var sendRequest = request.ToDomainRequest(user.Id);
        return _roomService.SendEventRequestAsync(sendRequest, HttpContext.RequestAborted).ToResponseAsync();
    }

    /// <summary>
    /// Sending code editor event to room.
    /// </summary>
    /// <param name="request">Request.</param>
    /// <returns>Ok message.</returns>
    [Authorize(policy: GulagSecurePolicy.Manager)]
    [HttpPost("event/codeEditor")]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(MessageResponse), StatusCodes.Status400BadRequest)]
    public Task<ActionResult> SendCodeEditorEvent(CodeEditorRoomEventApiRequest request)
    {
        var user = User.ToUser();
        if (user == null)
        {
            return Task.FromResult<ActionResult>(Unauthorized());
        }

        var sendRequest = request.ToDomainRequest(user.Id);
        return _roomService.SendEventRequestAsync(sendRequest, HttpContext.RequestAborted).ToResponseAsync();
    }

    /// <summary>
    /// Get analytics by room.
    /// </summary>
    /// <param name="id">Room id.</param>
    /// <returns>Analytics.</returns>
    [Authorize(policy: GulagSecurePolicy.Manager)]
    [HttpGet("{id:guid}/analytics")]
    [ProducesResponseType(typeof(Analytics), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(MessageResponse), StatusCodes.Status404NotFound)]
    public Task<ActionResult<Analytics>> GetAnalytics(Guid id)
    {
        return _roomService.GetAnalyticsAsync(new RoomAnalyticsRequest(id), HttpContext.RequestAborted).ToResponseAsync();
    }

    /// <summary>
    /// Get analytics  by room.
    /// </summary>
    /// <param name="id">Room id.</param>
    /// <returns>Analytics.</returns>
    [Authorize]
    [HttpGet("{id:guid}/analytics/summary")]
    [ProducesResponseType(typeof(AnalyticsSummary), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(MessageResponse), StatusCodes.Status404NotFound)]
    public Task<ActionResult<AnalyticsSummary>> GetAnalyticsSummary(Guid id)
    {
        var user = User.ToUser();
        if (user == null)
        {
            return Task.FromResult<ActionResult<AnalyticsSummary>>(Unauthorized());
        }

        var request = new RoomAnalyticsRequest(id);
        return _roomService.GetAnalyticsSummaryAsync(request, HttpContext.RequestAborted).ToResponseAsync();
    }

    [Authorize(policy: GulagSecurePolicy.Manager)]
    [HttpPatch("{id:guid}/close")]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(MessageResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(MessageResponse), StatusCodes.Status400BadRequest)]
    public Task<ActionResult> CloseRoom(Guid id)
    {
        return _roomService.CloseRoomAsync(id, HttpContext.RequestAborted).ToResponseAsync();
    }

    [Authorize(policy: GulagSecurePolicy.Manager)]
    [HttpPatch("{id:guid}/startReview")]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(MessageResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(MessageResponse), StatusCodes.Status400BadRequest)]
    public Task<ActionResult> StartReviewRoom(Guid id)
    {
        return _roomService.StartReviewRoomAsync(id, HttpContext.RequestAborted).ToResponseAsync();
    }
}
