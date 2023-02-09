using System.ComponentModel.DataAnnotations;
using System.Runtime.InteropServices.JavaScript;
using Interview.Domain.Users;
using Interview.Infrastructure.Users;
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
    public Task<IPagedList<User>> GetPage([Range(1, Int32.MaxValue)] int pageNumber)
    {
        return _userRepository.GetPage(pageNumber, 30);
    }

    [HttpGet(nameof(FindByNickname))]
    public Task<User?> FindByNickname(String nickname)
    {
        return _userRepository.FindByNicknameAsync(nickname);
    }
    
}