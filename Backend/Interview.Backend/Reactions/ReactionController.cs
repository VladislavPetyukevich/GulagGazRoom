using Interview.Backend.Auth;
using Interview.Backend.Shared;
using Interview.Domain.Reactions.Records;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using X.PagedList;

namespace Interview.Backend.Reactions
{
    [ApiController]
    [Route("[controller]")]
    public class ReactionController : ControllerBase
    {
        private readonly ReactionService _reactionService;

        public ReactionController(ReactionService reactionService)
        {
            _reactionService = reactionService;
        }

        [Authorize(policy: GulagSecurePolicy.User)]
        [HttpGet(nameof(GetPage))]
        [ProducesResponseType(typeof(IPagedList<ReactionDetail>), StatusCodes.Status200OK)]
        public Task<IPagedList<ReactionDetail>> GetPage([FromQuery] PageRequest request) =>
            _reactionService.GetPageAsync(request.PageNumber, request.PageSize, HttpContext.RequestAborted);
    }
}
