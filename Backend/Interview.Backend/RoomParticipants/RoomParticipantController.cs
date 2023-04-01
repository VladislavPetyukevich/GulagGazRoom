using Interview.Backend.Auth;
using Interview.Backend.Responses;
using Interview.Domain.RoomParticipants.Records;
using Interview.Domain.RoomParticipants.Records.Request;
using Interview.Domain.RoomParticipants.Records.Response;
using Interview.Domain.RoomParticipants.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Interview.Backend.RoomParticipants
{
    public class RoomParticipantController : ControllerBase
    {
        private readonly RoomParticipantService _roomParticipantService;

        public RoomParticipantController(RoomParticipantService roomParticipantService)
        {
            _roomParticipantService = roomParticipantService;
        }

        [Authorize(policy: GulagSecurePolicy.Manager)]
        [HttpPost(nameof(CreateParticipant))]
        [ProducesResponseType(typeof(RoomParticipantDetail), 200)]
        [ProducesResponseType(typeof(string), 404)]
        public Task<ActionResult<RoomParticipantDetail>> CreateParticipant(
            [FromBody] RoomParticipantCreateRequest request)
        {
            return _roomParticipantService.CreateParticipantAsync(request, HttpContext.RequestAborted).ToResponseAsync();
        }

        [Authorize(policy: GulagSecurePolicy.Manager)]
        [HttpPatch(nameof(ChangeParticipantStatus))]
        [ProducesResponseType(typeof(RoomParticipantDetail), 200)]
        [ProducesResponseType(typeof(string), 404)]
        public Task<ActionResult<RoomParticipantDetail>> ChangeParticipantStatus(
            [FromBody] RoomParticipantChangeStatusRequest request)
        {
            return _roomParticipantService.ChangeParticipantStatusAsync(request, HttpContext.RequestAborted).ToResponseAsync();
        }
    }
}
