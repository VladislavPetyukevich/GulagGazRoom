using Interview.Backend.Auth;
using Interview.Backend.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using X.PagedList;

namespace Interview.Backend.Questions;

[Authorize(policy: OAuthTwitchOptions.Policy)]
[ApiController]
[Route("[controller]")]
public class QuestionController : ControllerBase
{
    private readonly IQuestionRepository _questionRepository;

    public QuestionController(IQuestionRepository questionRepository)
    {
        _questionRepository = questionRepository;
    }

    [HttpGet(nameof(GetPage))]
    public Task<IPagedList<Question>> GetPage([FromQuery] PageRequest request)
    {
        return _questionRepository.GetPageAsync(request.PageNumber, request.PageSize);
    }

    [Authorize(Roles = RoleNameConstants.Admin)]
    [HttpPost(nameof(Create))]
    public Task<Question> Create(QuestionCreateRequest request)
    {
        return _questionRepository.CreateAsync(request);
    }
}
