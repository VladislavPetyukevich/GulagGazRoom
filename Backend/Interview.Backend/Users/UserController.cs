using CSharpFunctionalExtensions;
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

    private readonly UserService _userService;

    private readonly UserClaimService _userClaimService;

    public UserController(IUserRepository userRepository, UserService userService, UserClaimService userClaimService)
    {
        _userRepository = userRepository;
        _userService = userService;
        _userClaimService = userClaimService;
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
    [HttpGet(nameof(GetByRole))]
    public Task<List<User>> GetByRole(RoleNameType role)
    {
        return _userService.GetByRoleAsync(role);
    }

    [Authorize]
    [HttpGet(nameof(GetMe))]
    [ProducesResponseType(typeof(UserClaim), 200)]
    [ProducesResponseType(typeof(string), 400)]
    public async Task<ActionResult<UserClaim?>> GetMe()
    {
        var userClaimResult =
            await _userClaimService.ParseClaimsAsync(HttpContext.User.Claims, HttpContext.RequestAborted);

        if (userClaimResult.IsFailure)
        {
            return BadRequest(userClaimResult.Error);
        }

        return Ok(userClaimResult.Value);
    }
}
