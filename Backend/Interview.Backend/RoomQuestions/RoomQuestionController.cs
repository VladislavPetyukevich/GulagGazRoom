using Interview.Backend.Auth;
using Interview.Backend.Responses;
using Interview.Domain.RoomQuestions;
using Interview.Domain.RoomQuestions.Records;
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
        [ProducesResponseType(typeof(RoomQuestionDetail), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(MessageResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(MessageResponse), StatusCodes.Status404NotFound)]
        public Task<ActionResult<RoomQuestionDetail>> ChangeActiveQuestion(
            RoomQuestionChangeActiveRequest request)
        {
            return _roomQuestionService.ChangeActiveQuestionAsync(request, HttpContext.RequestAborted).ToResponseAsync();
        }

        [Authorize(policy: GulagSecurePolicy.Manager)]
        [HttpPost(nameof(Create))]
        [ProducesResponseType(typeof(RoomQuestionDetail), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(MessageResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(MessageResponse), StatusCodes.Status404NotFound)]
        public Task<ActionResult<RoomQuestionDetail>> Create(
            RoomQuestionCreateRequest request)
        {
            return _roomQuestionService.CreateAsync(request, HttpContext.RequestAborted).ToResponseAsync();
        }

        [Authorize]
        [HttpGet(nameof(GetRoomQuestions))]
        [ProducesResponseType(typeof(List<Guid>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(List<Guid>), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(MessageResponse), StatusCodes.Status404NotFound)]
        public Task<ActionResult<List<Guid>>> GetRoomQuestions([FromQuery] RoomQuestionsRequest request)
        {
            return _roomQuestionService.GetRoomQuestionsAsync(request).ToResponseAsync();
        }
    }
}
