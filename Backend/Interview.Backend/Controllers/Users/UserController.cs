using Interview.Backend.Shared;
using Interview.Domain.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TwitchLib.Api.Helix.Models.Users.GetUsers;
using X.PagedList;
using User = Interview.Domain.Users.User;

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


    [Authorize(policy: "user")]
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
    
    [Authorize(policy: "user")]
    [HttpGet(nameof(GetMe))]
    public Task<List<string>> GetMe()
    {
        var claims = HttpContext.User.Claims.Select(claim => claim.Value).ToList();
        return Task.FromResult(claims);
    }
}