using Interview.Backend.Shared;
using Interview.Domain.Users;
using Microsoft.AspNetCore.Mvc;
using X.PagedList;

namespace Interview.Backend.Controllers.Users;

[ApiController]
[Route("[controller]")]
public class UserController : ControllerBase
{
    private readonly IUserRepository _userRepository;

    public UserController(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    [HttpGet(nameof(GetPage))]
    public Task<IPagedList<User>> GetPage([FromQuery]PageRequest request)
    {
        return _userRepository.GetPage(request.PageNumber, request.PageSize);
    }

    [HttpGet(nameof(FindByNickname))]
    public Task<User?> FindByNickname(string nickname)
    {
        return _userRepository.FindByNicknameAsync(nickname);
    }
}
