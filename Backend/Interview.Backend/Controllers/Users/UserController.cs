using System.ComponentModel.DataAnnotations;
using Interview.Domain.Users;
using Interview.Infrastructure.Constants;
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
}