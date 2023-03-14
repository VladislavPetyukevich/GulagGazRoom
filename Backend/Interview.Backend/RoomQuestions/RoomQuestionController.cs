using Interview.Domain.RoomQuestions;
using Interview.Domain.RoomQuestions.Records.Response;
using Microsoft.AspNetCore.Mvc;

namespace Interview.Backend.RoomQuestions
{
    public class RoomQuestionController : ControllerBase
    {
        private readonly RoomQuestionService _roomQuestionService;

        public RoomQuestionController(RoomQuestionService roomQuestionService)
        {
            _roomQuestionService = roomQuestionService;
        }

        [HttpPost(nameof(ChangeActiveQuestion))]
        [ProducesResponseType(typeof(RoomQuestionChangeActiveRequest), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<ActionResult<RoomQuestionChangeActiveRequest?>> ChangeActiveQuestion(
            RoomQuestionChangeActiveRequest request)
        {
            var result = await _roomQuestionService.ChangeActiveQuestion(request);

            if (result.IsFailure)
            {
                return BadRequest();
            }

            return Ok(result.Value);
        }
    }
}
