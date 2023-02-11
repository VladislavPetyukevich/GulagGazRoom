using System.ComponentModel.DataAnnotations;
using Interview.Domain.Users;
using Interview.Infrastructure.Constants;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
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


    [Authorize(policy: "user")]
    [HttpGet(nameof(GetPage))]
    public Task<IPagedList<User>> GetPage([Range(1, int.MaxValue)] int pageNumber,
        [Range(1, PageProperty.DefaultSize)] int pageSize)
    {
        return _userRepository.GetPage(pageNumber, pageSize);
    }

    [HttpGet(nameof(FindByNickname))]
    public Task<User?> FindByNickname(string nickname)
    {
        return _userRepository.FindByNicknameAsync(nickname);
    }
    
    [Authorize(policy: "user")]
    [HttpGet(nameof(GetMe))]
    public Task<List<Claim>> GetMe()
    {
        List<Claim> claims = HttpContext.User.Claims.Select(claim => new Claim {Type = claim.Type, Name = claim.Value}).ToList();
        return Task.FromResult(claims);
    }

    public class Claim
    {
        public string Type { get; init; } = string.Empty;
        
        public string Name { get; init; } = string.Empty;
    }

}