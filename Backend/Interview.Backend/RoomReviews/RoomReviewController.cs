using System.Net;
using System.Net.Mime;
using Interview.Backend.Auth;
using Interview.Backend.Responses;
using Interview.Domain;
using Interview.Domain.RoomReviews;
using Interview.Domain.RoomReviews.Records;
using Interview.Domain.RoomReviews.Services;
using Interview.Domain.Rooms.Service.Records.Response.Page;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using X.PagedList;

namespace Interview.Backend.RoomReviews;

[Route("room-reviews")]
public class RoomReviewController : ControllerBase
{
    private readonly IRoomReviewService _roomReviewService;

    public RoomReviewController(IRoomReviewService roomReviewService)
    {
        _roomReviewService = roomReviewService;
    }

    /// <summary>
    /// Getting a room reviews page.
    /// </summary>
    /// <param name="request">Request.</param>
    /// <returns>Page.</returns>
    [Authorize]
    [HttpGet]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(List<RoomReviewDetail>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(MessageResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(MessageResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(MessageResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(MessageResponse), StatusCodes.Status500InternalServerError)]
    public Task<IPagedList<RoomReviewPageDetail>> FindPage([FromQuery] RoomReviewPageRequest request)
    {
        return _roomReviewService.FindPageAsync(request, HttpContext.RequestAborted);
    }

    /// <summary>
    /// Creating a review for a room.
    /// </summary>
    /// <param name="request">User Request.</param>
    /// <returns>Review details.</returns>
    [Authorize]
    [HttpPost]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(RoomReviewDetail), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(MessageResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(MessageResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(MessageResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(MessageResponse), StatusCodes.Status500InternalServerError)]
    public Task<ActionResult<RoomReviewDetail>> Create([FromBody] RoomReviewCreateRequest request)
    {
        var user = HttpContext.User.ToUser();

        if (user is null)
        {
            throw new AccessDeniedException("Current user not found");
        }

        return Task.FromResult<ActionResult<RoomReviewDetail>>(
            new ObjectResult(_roomReviewService.CreateAsync(request, user.Id, HttpContext.RequestAborted))
            {
                StatusCode = (int?)HttpStatusCode.Created,
            });
    }

    /// <summary>
    /// Update a review by id.
    /// </summary>
    /// <param name="id">Id review.</param>
    /// <param name="request">User Request.</param>
    /// <returns>Review details.</returns>
    [Authorize]
    [HttpPut("{id:guid}")]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(RoomReviewDetail), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(MessageResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(MessageResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(MessageResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(MessageResponse), StatusCodes.Status500InternalServerError)]
    public Task<RoomReviewDetail> Update([FromRoute] Guid id, [FromBody] RoomReviewUpdateRequest request)
    {
        return _roomReviewService.UpdateAsync(id, request, HttpContext.RequestAborted);
    }
}
