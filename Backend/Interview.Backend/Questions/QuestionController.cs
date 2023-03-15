using CSharpFunctionalExtensions;
using Interview.Backend.Auth;
using Interview.Backend.Shared;
using Interview.Domain.Questions.Records.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using X.PagedList;

namespace Interview.Backend.Questions;

[Authorize]
[ApiController]
[Route("[controller]")]
public class QuestionController : ControllerBase
{
    private readonly IQuestionRepository _questionRepository;
    private readonly QuestionService _questionService;

    public QuestionController(IQuestionRepository questionRepository, QuestionService questionService)
    {
        _questionRepository = questionRepository;
        _questionService = questionService;
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
        return _questionService.CreateAsync(request);
    }

    [Authorize(Roles = RoleNameConstants.Admin)]
    [HttpPut(nameof(Update))]
    [ProducesResponseType(typeof(Question), 200)]
    [ProducesResponseType(typeof(string), 404)]
    public async Task<IActionResult> Update(QuestionEditRequest request)
    {
        var result = await _questionService.UpdateAsync(request);
        if (result == null)
        {
            return NotFound($"Not found question with id = \"{request.Id}\"");
        }

        return Ok(result);
    }

    [Authorize]
    [HttpGet(nameof(GetById))]
    [ProducesResponseType(typeof(Question), 200)]
    [ProducesResponseType(typeof(string), 404)]
    public async Task<IActionResult> GetById([FromQuery] Guid id)
    {
        var questionItemResult = await _questionService.FindById(id);

        return questionItemResult.IsFailure
            ? NotFound(questionItemResult.Error)
            : Ok(questionItemResult.Value);
    }
}
