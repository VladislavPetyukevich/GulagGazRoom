using CSharpFunctionalExtensions;
using Interview.Backend.Auth;
using Interview.Backend.Responses;
using Interview.Backend.Shared;
using Interview.Domain.Questions.Records.Response;
using Interview.Domain.Users.Records;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using X.PagedList;

namespace Interview.Backend.Users;

[ApiController]
[Route("users")]
public class UserController : ControllerBase
{
    private readonly UserService _userService;

    private readonly UserClaimService _userClaimService;

    public UserController(UserService userService, UserClaimService userClaimService)
    {
        _userService = userService;
        _userClaimService = userClaimService;
    }

    [Authorize(policy: GulagSecurePolicy.Manager)]
    [HttpGet]
    [Produces("application/json")]
    [ProducesResponseType(typeof(IPagedList<UserDetail>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(MessageResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(MessageResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(MessageResponse), StatusCodes.Status500InternalServerError)]
    public Task<IPagedList<UserDetail>> FindPage([FromQuery] PageRequest request)
    {
        return _userService.FindPageAsync(request.PageNumber, request.PageSize, HttpContext.RequestAborted);
    }

    [Authorize(policy: GulagSecurePolicy.Manager)]
    [HttpGet("nickname/{nickname}")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(IPagedList<UserDetail>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(MessageResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(MessageResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(MessageResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(MessageResponse), StatusCodes.Status500InternalServerError)]
    public Task<ActionResult<UserDetail>> FindByNickname([FromRoute] string nickname)
    {
        return _userService.FindByNicknameAsync(nickname, HttpContext.RequestAborted).ToResponseAsync();
    }

    [Authorize(policy: GulagSecurePolicy.Manager)]
    [HttpGet("role/{role}")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(IPagedList<UserDetail>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(MessageResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(MessageResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(MessageResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(MessageResponse), StatusCodes.Status500InternalServerError)]
    public Task<IPagedList<UserDetail>> FindByRole([FromQuery] PageRequest pageRequest, [FromRoute] RoleNameType role)
    {
        return _userService.FindByRoleAsync(pageRequest.PageNumber, pageRequest.PageSize, role, HttpContext.RequestAborted);
    }

    [Authorize]
    [HttpGet("self")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(UserClaim), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(MessageResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(MessageResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(MessageResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(MessageResponse), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<UserClaim?>> GetMyself()
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
