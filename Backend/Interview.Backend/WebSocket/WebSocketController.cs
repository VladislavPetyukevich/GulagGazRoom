using System.Net.WebSockets;
using CSharpFunctionalExtensions;
using Interview.Backend.Auth;
using Interview.Backend.WebSocket.ConnectListener;
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
    private readonly WebSocketConnectListenerSource _webSocketConnectListenerSource;

    public WebSocketController(UserByRoomEventSubscriber userByRoomEventSubscriber, RoomService roomService, WebSocketConnectListenerSource webSocketConnectListenerSource)
    {
        _roomService = roomService;
        _webSocketConnectListenerSource = webSocketConnectListenerSource;
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

        var ct = HttpContext.RequestAborted;
        using var webSocket = await HttpContext.WebSockets.AcceptWebSocketAsync();

        (Guid RoomId, Guid UserId, string TwitchChannel)? connectionDetail = null;
        try
        {
            if (!HttpContext.Request.Query.TryGetValue("roomId", out var roomIdentityString))
            {
                await webSocket.CloseAsync(WebSocketCloseStatus.InvalidPayloadData, "Invalid room details", ct);
                return;
            }

            if (!Guid.TryParse(roomIdentityString, out var roomIdentity))
            {
                await webSocket.CloseAsync(WebSocketCloseStatus.InvalidPayloadData, "Invalid room details", ct);
                return;
            }

            var (_, isFailure, dbRoom) = await _roomService.PrepareRoomAsync(roomIdentity, user.Id, ct);

            if (isFailure || dbRoom == null)
            {
                await webSocket.CloseAsync(WebSocketCloseStatus.InvalidPayloadData, "Invalid room details", ct);
                return;
            }

            var task = _userByRoomEventSubscriber.SubscribeAsync(dbRoom.Id, webSocket, ct);
            _webSocketConnectListenerSource.Connect(dbRoom.Id, user.Id, dbRoom.TwitchChannel);
            connectionDetail = (dbRoom.Id, user.Id, dbRoom.TwitchChannel);
            await task;
            try
            {
                await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, string.Empty, ct);
            }
            catch
            {
                // ignore
            }
        }
        catch (Exception e)
        {
            await webSocket.CloseAsync(WebSocketCloseStatus.InvalidPayloadData, e.Message, ct);
        }
        finally
        {
            if (connectionDetail.HasValue)
            {
                var (roomId, userId, twitchChannel) = connectionDetail.Value;
                _webSocketConnectListenerSource.Disconnect(roomId, userId, twitchChannel);
            }
        }
    }
}
