using Interview.Backend.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using X.PagedList;

namespace Interview.Backend.Rooms;

[ApiController]
[Route("[controller]")]
public class RoomController : ControllerBase
{
    private readonly IRoomRepository _roomRepository;

    public RoomController(IRoomRepository userRepository)
    {
        _roomRepository = userRepository;
    }

    [HttpGet(nameof(GetPage))]
    public Task<IPagedList<Room>> GetPage([FromQuery] PageRequest request)
    {
        return _roomRepository.GetPage(request.PageNumber, request.PageSize);
    }

    [HttpPost(nameof(Create))]
    public Task Create(Room room)
    {
        return _roomRepository.CreateAsync(room);
    }

    [Authorize(Roles = RoleNameConstants.Admin)]
    [HttpPost(nameof(StartRoom))]
    public async Task<IActionResult> StartRoom(Guid id)
    {
        var room = await _roomRepository.FindByIdAsync(id);

        if (room == null)
        {
            return NotFound($"Not found room by id = \"{id}\"");
        }

        // TODO make jobs [reactionSender, chatMessaging]

        // TODO modify state room
        return Ok();
    }
}
