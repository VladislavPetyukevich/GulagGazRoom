using Interview.Backend.Auth;
using Interview.Backend.Shared;
using Interview.Domain.Rooms.Service;
using Interview.Domain.Rooms.Service.Records.Request;
using Interview.Domain.Rooms.Service.Records.Response.Page;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using X.PagedList;

namespace Interview.Backend.Rooms;

[Authorize(policy: OAuthTwitchOptions.Policy)]
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

    [HttpGet(nameof(GetPage))]
    public Task<IPagedList<RoomPageDetail>> GetPage([FromQuery] PageRequest request)
    {
        return _roomRepository.GetDetailedPageAsync(request.PageNumber, request.PageSize);
    }

    [HttpGet(nameof(GetById))]
    [ProducesResponseType(typeof(Room), 200)]
    [ProducesResponseType(typeof(string), 404)]
    public async Task<IActionResult> GetById([FromQuery] Guid id)
    {
        var result = await _roomRepository.FindByIdAsync(id);
        if (result == null)
        {
            return NotFound($"Not found user with id = \'{id}\'");
        }

        return Ok(result);
    }

    [Authorize(Roles = RoleNameConstants.Admin)]
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

    [Authorize(Roles = RoleNameConstants.Admin)]
    [HttpPatch]
    [ProducesResponseType(typeof(Guid), 200)]
    [ProducesResponseType(typeof(string), 400)]
    public async Task<IActionResult> PatchUpdate([FromQuery] Guid? id, [FromBody] RoomPatchUpdateRequest room)
    {
        var updatedRoomResult = await _roomService.PatchUpdate(id, room);

        return updatedRoomResult.IsFailure
            ? BadRequest(updatedRoomResult.Error)
            : Ok(updatedRoomResult);
    }
}
