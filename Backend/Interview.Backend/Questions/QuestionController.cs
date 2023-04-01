using Interview.Backend.Auth;
using Interview.Backend.Shared;
using Interview.Domain.Questions.Records.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using X.PagedList;

namespace Interview.Backend.Questions;

[ApiController]
[Route("questions")]
public class QuestionController : ControllerBase
{
    private readonly QuestionService _questionService;

    public QuestionController(QuestionService questionService)
    {
        _questionService = questionService;
    }

    /// <summary>
    /// Getting a Question page.
    /// </summary>
    /// <param name="request">Page Parameters.</param>
    /// <returns>A page of questions with metadata about the pages.</returns>
    [Authorize]
    [HttpGet]
    [Produces("application/json")]
    [ProducesResponseType(typeof(IPagedList<QuestionItem>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(string), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
    public Task<IPagedList<QuestionItem>> GetPage([FromQuery] PageRequest request)
    {
        return _questionService.FindPageAsync(request.PageSize, request.PageNumber, HttpContext.RequestAborted);
    }

    /// <summary>
    /// Getting a question by ID.
    /// </summary>
    /// <param name="id">Question ID.</param>
    /// <returns>The found object of the question.</returns>
    [Authorize]
    [HttpGet("/{id:guid}")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(QuestionItem), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(string), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<QuestionItem?>> GetById(Guid id)
    {
        var questionItemResult = await _questionService.FindById(id, HttpContext.RequestAborted);

        return questionItemResult.IsFailure
            ? NotFound(questionItemResult.Error)
            : Ok(questionItemResult.Value);
    }

    /// <summary>
    /// Creating a new question.
    /// </summary>
    /// <param name="request">The object with the question data for which you need to create.</param>
    /// <returns>The object of the new question.</returns>
    [Authorize(policy: GulagSecurePolicy.Manager)]
    [HttpPost]
    [Produces("application/json")]
    [ProducesResponseType(typeof(QuestionItem), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<QuestionItem?>> Create(QuestionCreateRequest request)
    {
        var createdQuestionResult = await _questionService.CreateAsync(request, HttpContext.RequestAborted);

        if (createdQuestionResult.IsFailure)
        {
            return BadRequest(createdQuestionResult.Error);
        }

        return Ok(createdQuestionResult.Value);
    }

    /// <summary>
    /// Updating the question by ID.
    /// </summary>
    /// <param name="id">ID of the of question.</param>
    /// <param name="request">The object with the question data for which you need to update.</param>
    /// <returns>Updated question object.</returns>
    [Authorize(policy: GulagSecurePolicy.Manager)]
    [HttpPut("/{id:guid}")]
    [ProducesResponseType(typeof(QuestionItem), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(string), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
    [Produces("application/json")]
    public async Task<ActionResult<QuestionItem?>> Update(Guid id, QuestionEditRequest request)
    {
        var result = await _questionService.UpdateAsync(id, request, HttpContext.RequestAborted);

        if (result.IsFailure)
        {
            return NotFound(result.Error);
        }

        return Ok(result);
    }
}
