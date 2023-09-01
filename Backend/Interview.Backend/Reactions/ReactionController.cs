using Interview.Backend.Auth;
using Interview.Backend.Responses;
using Interview.Domain;
using Interview.Domain.Reactions.Records;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using X.PagedList;

namespace Interview.Backend.Reactions;

[ApiController]
[Route("reactions")]
public class ReactionController : ControllerBase
{
    private readonly ReactionService _reactionService;

    public ReactionController(ReactionService reactionService)
    {
        _reactionService = reactionService;
    }

    /// <summary>
    /// Getting a list of reactions.
    /// </summary>
    /// <param name="request">Page Parameters.</param>
    /// <returns>List of reactions.</returns>
    [HttpGet]
    [Authorize(policy: GulagSecurePolicy.User)]
    [ProducesResponseType(typeof(IPagedList<ReactionDetail>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(MessageResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(MessageResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(MessageResponse), StatusCodes.Status500InternalServerError)]
    public Task<IPagedList<ReactionDetail>> GetPage([FromQuery] PageRequest request) =>
        _reactionService.GetPageAsync(request.PageNumber, request.PageSize, HttpContext.RequestAborted);
}
