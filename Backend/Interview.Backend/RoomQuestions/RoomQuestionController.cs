using Interview.Backend.Auth;
using Interview.Domain.RoomQuestions;
using Interview.Domain.RoomQuestions.Records.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Interview.Backend.RoomQuestions
{
    [ApiController]
    [Route("[controller]")]
    public class RoomQuestionController : ControllerBase
    {
        private readonly RoomQuestionService _roomQuestionService;

        public RoomQuestionController(RoomQuestionService roomQuestionService)
        {
            _roomQuestionService = roomQuestionService;
        }

        [Authorize(policy: GulagSecurePolicy.Manager)]
        [HttpPost(nameof(ChangeActiveQuestion))]
        [ProducesResponseType(typeof(RoomQuestionChangeActiveRequest), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<ActionResult<RoomQuestionChangeActiveRequest?>> ChangeActiveQuestion(
            RoomQuestionChangeActiveRequest request)
        {
            var result = await _roomQuestionService.ChangeActiveQuestionAsync(request);

            if (result.IsFailure)
            {
                return BadRequest();
            }

            return Ok(result.Value);
        }
    }
}
