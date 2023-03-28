using Interview.Backend.Auth;
using Interview.Domain.RoomQuestions;
using Interview.Domain.RoomQuestions.Records;
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
            var result = await _roomQuestionService.ChangeActiveQuestionAsync(request, HttpContext.RequestAborted);

            if (result.IsFailure)
            {
                return BadRequest(result.Error);
            }

            return Ok(result.Value);
        }

        [Authorize(policy: GulagSecurePolicy.Manager)]
        [HttpPost(nameof(Create))]
        [ProducesResponseType(typeof(RoomQuestionChangeActiveRequest), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<ActionResult<RoomQuestionChangeActiveRequest?>> Create(
            RoomQuestionCreateRequest request)
        {
            var result = await _roomQuestionService.CreateAsync(request, HttpContext.RequestAborted);

            if (result.IsFailure)
            {
                return BadRequest(result.Error);
            }

            return Ok(result.Value);
        }

        [Authorize]
        [HttpGet(nameof(GetRoomQuestions))]
        [ProducesResponseType(typeof(List<Guid>), 200)]
        [ProducesResponseType(typeof(string), 404)]
        public async Task<IActionResult> GetRoomQuestions([FromQuery] RoomQuestionsRequest request)
        {
            var result = await _roomQuestionService.GetRoomQuestionsAsync(request);
            if (result.IsFailure)
            {
                return BadRequest(result.Error);
            }

            return Ok(result.Value);
        }
    }
}
