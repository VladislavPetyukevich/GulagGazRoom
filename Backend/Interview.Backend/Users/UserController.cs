using Interview.Backend.Auth;
using Interview.Backend.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using X.PagedList;

namespace Interview.Backend.Users;

[ApiController]
[Route("[controller]")]
public class UserController : ControllerBase
{
    private readonly IUserRepository _userRepository;

    public UserController(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    [Authorize]
    [HttpGet(nameof(GetPage))]
    public Task<IPagedList<User>> GetPage([FromQuery] PageRequest request)
    {
        return _userRepository.GetPageAsync(request.PageNumber, request.PageSize);
    }

    [Authorize]
    [HttpGet(nameof(FindByNickname))]
    public Task<User?> FindByNickname(string nickname)
    {
        return _userRepository.FindByNicknameAsync(nickname);
    }

    [Authorize]
    [HttpGet(nameof(GetMe))]
    public Task<User?> GetMe()
    {
        return Task.FromResult(HttpContext.User.ToUser());
    }
}
