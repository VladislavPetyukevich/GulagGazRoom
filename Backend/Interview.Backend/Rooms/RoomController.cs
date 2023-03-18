using Interview.Backend.Auth;
using Interview.Backend.Shared;
using Interview.Domain.Rooms.Service;
using Interview.Domain.Rooms.Service.Records.Request;
using Interview.Domain.Rooms.Service.Records.Response.Page;
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
    [ProducesResponseType(typeof(Room), 200)]
    [ProducesResponseType(typeof(string), 404)]
    public async Task<IActionResult> GetById([FromQuery] Guid id)
    {
        var room = await _roomRepository.GetByIdAsync(id);
        var notFoundMessage = room?.GetNotFoundMessage("room", id);
        if (!string.IsNullOrEmpty(notFoundMessage))
        {
            return NotFound(notFoundMessage);
        }

        return Ok(room);
    }

    [Authorize(policy: GulagSecurePolicy.Manager)]
    [HttpPost(nameof(Create))]
    [ProducesResponseType(typeof(Guid), 201)]
    [ProducesResponseType(typeof(string), 400)]
    public async Task<IActionResult> Create([FromBody] RoomCreateRequest room)
    {
        var newRoomResult = await _roomService.CreateAsync(room);
        if (newRoomResult.IsFailure)
        {
            return BadRequest(newRoomResult.Error);
        }

        return Created(string.Empty, newRoomResult.Value.Id);
    }

    [Authorize(policy: GulagSecurePolicy.Manager)]
    [HttpPatch]
    [ProducesResponseType(typeof(Guid), 200)]
    [ProducesResponseType(typeof(string), 400)]
    public async Task<IActionResult> PatchUpdate([FromQuery] Guid? id, [FromBody] RoomPatchUpdateRequest room)
    {
        var updatedRoomResult = await _roomService.PatchUpdate(id, room);

        return updatedRoomResult.IsFailure
            ? BadRequest(updatedRoomResult.Error)
            : Ok(updatedRoomResult.Value);
    }

    [Authorize(policy: GulagSecurePolicy.Manager)]
    [HttpPost(nameof(SendGasEvent))]
    [ProducesResponseType(typeof(string), 200)]
    [ProducesResponseType(typeof(string), 400)]
    public async Task<ActionResult<string?>> SendGasEvent(SendGasRoomEventApiRequest request)
    {
        var user = User.ToUser();
        if (user == null)
        {
            return Unauthorized();
        }

        var sendRequest = request.ToDomainRequest(user.Id);
        var result = await _roomService.SendGasEventAsync(sendRequest, HttpContext.RequestAborted);
        if (result.IsFailure)
        {
            return BadRequest(result.Error);
        }

        return Ok();
    }
}
