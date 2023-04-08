using Interview.Backend.Auth;
using Interview.Backend.Responses;
using Interview.Backend.Shared;
using Interview.Domain.Rooms.Service;
using Interview.Domain.Rooms.Service.Records.Request;
using Interview.Domain.Rooms.Service.Records.Response;
using Interview.Domain.Rooms.Service.Records.Response.Detail;
using Interview.Domain.Rooms.Service.Records.Response.RoomStates;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using X.PagedList;

namespace Interview.Backend.Rooms;

[ApiController]
[Route("[controller]")]
public class RoomController : ControllerBase
{
    private readonly IRoomRepository _roomRepository;
    private readonly RoomService _roomService;

    public RoomController(IRoomRepository userRepository, RoomService roomService)
    {
        _roomRepository = userRepository;
        _roomService = roomService;
    }

    [Authorize]
    [HttpGet(nameof(GetPage))]
    public Task<IPagedList<RoomDetail>> GetPage([FromQuery] PageRequest request)
    {
        return _roomRepository.GetDetailedPageAsync(request.PageNumber, request.PageSize);
    }

    [Authorize]
    [HttpGet(nameof(GetById))]
    [ProducesResponseType(typeof(Room), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(MessageResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById([FromQuery] Guid id)
    {
        var room = await _roomRepository.GetByIdAsync(id);
        return room is null ? NotFound(new MessageResponse
        {
            Message = $"Not found room by id {id}",
        }) : Ok(room);
    }

    [Authorize]
    [HttpGet(nameof(GetRoomState))]
    [ProducesResponseType(typeof(RoomState), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(MessageResponse), StatusCodes.Status404NotFound)]
    public Task<ActionResult<RoomState>> GetRoomState([FromQuery] Guid id)
    {
        return _roomService.GetRoomStateAsync(id).ToResponseAsync();
    }

    [Authorize(policy: GulagSecurePolicy.Manager)]
    [HttpPost(nameof(Create))]
    [ProducesResponseType(typeof(Guid), 201)]
    [ProducesResponseType(typeof(MessageResponse), StatusCodes.Status400BadRequest)]
    public Task<ActionResult<Room>> Create([FromBody] RoomCreateRequest room)
    {
        return _roomService.CreateAsync(room).ToResponseAsync();
    }

    [Authorize(policy: GulagSecurePolicy.Manager)]
    [HttpPatch]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(MessageResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> PatchUpdate([FromQuery] Guid? id, [FromBody] RoomPatchUpdateRequest room)
    {
        var updatedRoomResult = await _roomService.PatchUpdate(id, room);

        return updatedRoomResult.IsFailure
            ? BadRequest(updatedRoomResult.Error)
            : Ok(updatedRoomResult.Value);
    }

    [Authorize(policy: GulagSecurePolicy.Manager)]
    [HttpPost(nameof(SendGasEvent))]
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
        return _roomService.SendGasEventAsync(sendRequest, HttpContext.RequestAborted).ToResponseAsync();
    }

    [Authorize(policy: GulagSecurePolicy.Manager)]
    [HttpGet("{id:guid}/analytics")]
    [ProducesResponseType(typeof(Analytics), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(MessageResponse), StatusCodes.Status404NotFound)]
    public Task<ActionResult<Analytics>> GetAnalytics(Guid id)
    {
        return _roomService.GetAnalyticsAsync(new RoomAnalyticsRequest(id), HttpContext.RequestAborted).ToResponseAsync();
    }

    [Authorize]
    [HttpGet("{id:guid}/analytics/summary")]
    [ProducesResponseType(typeof(Analytics), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(MessageResponse), StatusCodes.Status404NotFound)]
    public Task<ActionResult<Analytics>> GetAnalyticsSummary(Guid id)
    {
        var user = User.ToUser();
        if (user == null)
        {
            return Task.FromResult<ActionResult<Analytics>>(Unauthorized());
        }

        var request = new RoomAnalyticsRequest(id, new[] { user.Id });
        return _roomService.GetAnalyticsAsync(request, HttpContext.RequestAborted).ToResponseAsync();
    }
}
