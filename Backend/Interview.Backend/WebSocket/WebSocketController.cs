using System.Net.WebSockets;
using Interview.Backend.Auth;
using Interview.Backend.WebSocket.UserByRoom;
using Interview.Domain.Rooms.Service;
using Microsoft.AspNetCore.Mvc;

namespace Interview.Backend.WebSocket;

[ApiController]
[Route("[controller]")]
public class WebSocketController : ControllerBase
{
    private readonly RoomService _roomService;
    private readonly UserByRoomEventSubscriber _userByRoomEventSubscriber;

    public WebSocketController(UserByRoomEventSubscriber userByRoomEventSubscriber, RoomService roomService)
    {
        _roomService = roomService;

        _userByRoomEventSubscriber = userByRoomEventSubscriber;
    }

    [Route("/ws")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task ExecuteWebSocket()
    {
        if (!HttpContext.WebSockets.IsWebSocketRequest)
        {
            HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
            return;
        }

        var user = HttpContext.User.ToUser();
        if (user == null)
        {
            HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
            return;
        }

        using var webSocket = await HttpContext.WebSockets.AcceptWebSocketAsync();

        try
        {
            if (!HttpContext.Request.Query.TryGetValue("roomId", out var roomIdentityString))
            {
                await webSocket.CloseAsync(WebSocketCloseStatus.InvalidPayloadData, "Invalid room details", CancellationToken.None);
                return;
            }

            if (!Guid.TryParse(roomIdentityString, out var roomIdentity))
            {
                await webSocket.CloseAsync(WebSocketCloseStatus.InvalidPayloadData, "Invalid room details", CancellationToken.None);
                return;
            }

            var resultRoom = await _roomService.PrepareRoomAsync(roomIdentity, user.Id);

            if (resultRoom.IsFailure || resultRoom.Value == null)
            {
                await webSocket.CloseAsync(WebSocketCloseStatus.InvalidPayloadData, "Invalid room details", CancellationToken.None);
                return;
            }

            await _userByRoomEventSubscriber.SubscribeAsync(resultRoom.Value.Id, webSocket);
        }
        catch (Exception e)
        {
            await webSocket.CloseAsync(WebSocketCloseStatus.InvalidPayloadData, e.Message, CancellationToken.None);
        }
    }
}
