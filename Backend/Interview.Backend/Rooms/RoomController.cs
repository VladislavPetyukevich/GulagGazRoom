using Interview.Backend.Shared;
using Interview.Domain.Rooms.Service;
using Microsoft.AspNetCore.Mvc;
using X.PagedList;

namespace Interview.Backend.Controllers.Rooms;

[ApiController]
[Route("[controller]")]
public class RoomController : ControllerBase
{
    private readonly IRoomRepository _roomRepository;
    private readonly RoomService _roomService;

    public RoomController(IRoomRepository userRepository)
    {
        _roomRepository = userRepository;
        _roomService = roomService;
    }

    [HttpGet(nameof(GetPage))]
    public Task<IPagedList<Room>> GetPage([FromQuery] PageRequest request)
    {
        return _roomRepository.GetPage(request.PageNumber, request.PageSize);
    }

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
}
