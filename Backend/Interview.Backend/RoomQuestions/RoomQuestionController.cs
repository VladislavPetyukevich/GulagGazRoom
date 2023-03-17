using Interview.Backend.Auth;
using Interview.Domain.RoomQuestions;
using Interview.Domain.RoomQuestions.Records;
using Interview.Domain.RoomQuestions.Records.Response;
using Interview.Domain.RoomQuestions.Records.Response.Response;
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
                return BadRequest(result.Error);
            }

            return Ok(result.Value);
        }

        [Authorize(policy: GulagSecurePolicy.Manager)]
        [HttpPost(nameof(Create))]
        [ProducesResponseType(typeof(RoomQuestionCreateRequest), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<ActionResult<RoomQuestionCreateRequest?>> Create(
            RoomQuestionCreateRequest request)
        {
            var result = await _roomQuestionService.Create(request);

            if (result.IsFailure)
            {
                return BadRequest(result.Error);
            }

            return Ok(result.Value);
        }

        [Authorize(policy: GulagSecurePolicy.User)]
        [HttpGet(nameof(GetActive))]
        [ProducesResponseType(typeof(RoomQuestionDetail), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<ActionResult<RoomQuestionDetail?>> GetActive([FromQuery] Guid room)
        {
            var result = await _roomQuestionService.GetActive(room);

            if (result.IsFailure)
            {
                return BadRequest(result.Error);
            }

            return Ok(result.Value);
        }
    }
}
