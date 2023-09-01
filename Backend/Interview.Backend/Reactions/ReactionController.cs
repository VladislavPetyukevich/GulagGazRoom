using Interview.Backend.Auth;
using Interview.Backend.Responses;
using Interview.Domain;
using Interview.Domain.Questions.Records.Response;
using Interview.Domain.Reactions.Records;
using Interview.Domain.Reactions.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using X.PagedList;

namespace Interview.Backend.Reactions;

[ApiController]
[Route("reactions")]
public class ReactionController : ControllerBase
{
    private readonly IReactionService _reactionService;

    public ReactionController(IReactionService reactionService)
    {
        _reactionService = reactionService;
    }

    /// <summary>
    /// Getting a list of reactions.
    /// </summary>
    /// <param name="request">Page Parameters.</param>
    /// <returns>List of reactions.</returns>
    [HttpGet]
    [Authorize]
    [ProducesResponseType(typeof(IPagedList<ReactionDetail>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(MessageResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(MessageResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(MessageResponse), StatusCodes.Status500InternalServerError)]
    public Task<IPagedList<ReactionDetail>> GetPage([FromQuery] PageRequest request) =>
        _reactionService.FindPageAsync(request.PageNumber, request.PageSize, HttpContext.RequestAborted);
}
