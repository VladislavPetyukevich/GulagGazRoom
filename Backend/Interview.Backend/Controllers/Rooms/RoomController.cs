using System.ComponentModel.DataAnnotations;
using Interview.Domain.Rooms;
using Interview.Infrastructure.Constants;
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
    public Task<IPagedList<Room>> GetPage([Range(1, int.MaxValue)] int pageNumber, 
        [Range(1, PageProperty.DefaultSize)] int pageSize)
    {
        return _roomRepository.GetPage(pageNumber, pageSize);
    }

    [HttpPost(nameof(Create))]
    public Task Create(Room room)
    {
        return _roomRepository.CreateAsync(room);
    }
}