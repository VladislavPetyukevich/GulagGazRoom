using Interview.Backend.Auth;
using Interview.Backend.Responses;
using Interview.Domain.RoomQuestions;
using Interview.Domain.RoomQuestions.Records;
using Interview.Domain.RoomQuestions.Records.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Interview.Backend.RoomQuestions;

[ApiController]
[Route("room-questions")]
public class RoomQuestionController : ControllerBase
{
    private readonly RoomQuestionService _roomQuestionService;

    public RoomQuestionController(RoomQuestionService roomQuestionService)
    {
        _roomQuestionService = roomQuestionService;
    }

    /// <summary>
    /// Changing the current question.
    /// </summary>
    /// <param name="request">User Request.</param>
    /// <returns>Data of the current question, room, status.</returns>
    [Authorize(policy: SecurePolicy.Manager)]
    [HttpPut("active-question")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(RoomQuestionDetail), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(MessageResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(MessageResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(MessageResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(MessageResponse), StatusCodes.Status500InternalServerError)]
    public Task<ActionResult<RoomQuestionDetail>> ChangeActiveQuestion(RoomQuestionChangeActiveRequest request)
    {
        return _roomQuestionService.ChangeActiveQuestionAsync(request, HttpContext.RequestAborted).ToResponseAsync();
    }

    /// <summary>
    /// Creating a question in a room.
    /// </summary>
    /// <param name="request">User Request.</param>
    /// <returns>Data of the current question, room, status.</returns>
    [Authorize(policy: SecurePolicy.Manager)]
    [HttpPost]
    [Produces("application/json")]
    [ProducesResponseType(typeof(RoomQuestionDetail), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(MessageResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(MessageResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(MessageResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(MessageResponse), StatusCodes.Status500InternalServerError)]
    public Task<ActionResult<RoomQuestionDetail>> Create(RoomQuestionCreateRequest request)
    {
        return _roomQuestionService.CreateAsync(request, HttpContext.RequestAborted).ToResponseAsync();
    }

    /// <summary>
    /// Getting a page with room questions.
    /// </summary>
    /// <param name="request">User Request.</param>
    /// <returns>The page with the questions of the room.</returns>
    [Authorize]
    [HttpGet]
    [Produces("application/json")]
    [ProducesResponseType(typeof(List<Guid>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(MessageResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(MessageResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(MessageResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(MessageResponse), StatusCodes.Status500InternalServerError)]
    public Task<ActionResult<List<Guid>>> FindRoomQuestions([FromQuery] RoomQuestionsRequest request)
    {
        return _roomQuestionService.FindRoomQuestionsAsync(request).ToResponseAsync();
    }
}
