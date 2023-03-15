using Interview.Backend.Auth;
using Interview.Backend.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using X.PagedList;

namespace Interview.Backend.Users;

[Authorize]
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
    public Task<IPagedList<User>> GetPage([FromQuery] PageRequest request)
    {
        return _userRepository.GetPageAsync(request.PageNumber, request.PageSize);
    }

    [HttpGet(nameof(FindByNickname))]
    public Task<User?> FindByNickname(string nickname)
    {
        return _userRepository.FindByNicknameAsync(nickname);
    }

    [Authorize(Roles = RoleNameConstants.User)]
    [HttpGet(nameof(GetMe))]
    public Task<List<string>> GetMe()
    {
        var claims = HttpContext.User.Claims.Select(claim => claim.Value).ToList();
        return Task.FromResult(claims);
    }
}
