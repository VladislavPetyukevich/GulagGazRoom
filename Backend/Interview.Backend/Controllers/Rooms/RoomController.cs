using System.ComponentModel.DataAnnotations;
using Interview.Domain.Rooms;
using Microsoft.AspNetCore.Mvc;
using X.PagedList;

namespace Interview.Backend.Controllers.Rooms;

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
    public Task<IPagedList<Room>> GetPage([Range(1, Int32.MaxValue)] int pageNumber)
    {
        return _roomRepository.GetPage(pageNumber, 30);
    }

    [HttpPost(nameof(Create))]
    public Task Create(Room room)
    {
        return _roomRepository.CreateAsync(room);
    }

}