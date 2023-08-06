using Interview.Backend.Auth;
using Interview.Backend.Responses;
using Interview.Domain;
using Interview.Domain.Events;
using Interview.Domain.Events.Service;
using Interview.Domain.Events.Service.Create;
using Interview.Domain.Events.Service.FindPage;
using Interview.Domain.Events.Service.Update;
using Interview.Domain.Questions.Records.Response;
using Interview.Domain.Rooms.Service.Records.Request;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using X.PagedList;

namespace Interview.Backend.AppEvents;

[ApiController]
[Route("event")]
public class EventController : ControllerBase
{
    /// <summary>
    /// Getting a AppEvent page.
    /// </summary>
    /// <param name="request">Page Parameters.</param>
    /// <param name="service">Repository.</param>
    /// <returns>A page of questions with metadata about the pages.</returns>
    [Authorize(policy: SecurePolicy.Manager)]
    [HttpGet]
    [Produces("application/json")]
    [ProducesResponseType(typeof(IPagedList<QuestionItem>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(MessageResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(MessageResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(MessageResponse), StatusCodes.Status500InternalServerError)]
    public Task<IPagedList<AppEventItem>> GetPage([FromQuery] PageRequest request, [FromServices] AppEventService service)
    {
        return service.FindPageAsync(request.PageNumber, request.PageSize, HttpContext.RequestAborted);
    }

    /// <summary>
    /// Getting a Event by ID.
    /// </summary>
    /// <param name="id">Id.</param>
    /// <param name="service">Service.</param>
    /// <returns>Room.</returns>
    [Authorize(policy: SecurePolicy.Manager)]
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(AppEventItem), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(MessageResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id, [FromServices] AppEventService service)
    {
        var room = await service.FindByIdAsync(id, HttpContext.RequestAborted);
        return room is null ? NotFound(new MessageResponse
        {
            Message = $"Not found event by id {id}",
        }) : Ok(room);
    }

    /// <summary>
    /// Getting a Event by Type.
    /// </summary>
    /// <param name="type">Id.</param>
    /// <param name="service">Service.</param>
    /// <returns>Room.</returns>
    [Authorize(policy: SecurePolicy.Manager)]
    [HttpGet("{type}")]
    [ProducesResponseType(typeof(AppEventItem), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(MessageResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(string type, [FromServices] AppEventService service)
    {
        var room = await service.FindByTypeAsync(type, HttpContext.RequestAborted);
        return room is null ? NotFound(new MessageResponse
        {
            Message = $"Not found event by type {type}",
        }) : Ok(room);
    }

    /// <summary>
    /// Creating a new event.
    /// </summary>
    /// <param name="request">Create request.</param>
    /// <param name="service">Service.</param>
    /// <returns>Created room.</returns>
    [Authorize(policy: SecurePolicy.Manager)]
    [HttpPost("")]
    [ProducesResponseType(typeof(Guid), 201)]
    [ProducesResponseType(typeof(MessageResponse), StatusCodes.Status400BadRequest)]
    public Task<ActionResult<Guid>> Create([FromBody] AppEventCreateRequest request, [FromServices] AppEventService service)
    {
        return service.CreateAsync(request, HttpContext.RequestAborted).ToResponseAsync();
    }

    /// <summary>
    /// Update room.
    /// </summary>
    /// <param name="id">Event id.</param>
    /// <param name="request">Request.</param>
    /// <param name="service">Service.</param>
    /// <returns>Ok message.</returns>
    [Authorize(policy: SecurePolicy.Manager)]
    [HttpPatch("{id:guid}")]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(MessageResponse), StatusCodes.Status400BadRequest)]
    public Task<ActionResult<AppEventItem>> PatchUpdate(Guid id, [FromBody] AppEventUpdateRequest request, [FromServices] AppEventService service)
    {
        return service.UpdateAsync(id, request, HttpContext.RequestAborted).ToResponseAsync();
    }
}
