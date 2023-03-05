using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Interview.Backend.Auth;
using Interview.Domain.Events;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;

namespace Interview.Backend.Controllers.WebSocket;

// [AllowAnonymous]
[ApiController]
[Route("[controller]")]
public class WebSocketController : ControllerBase
{
    private readonly IRoomRepository _roomRepository;
    private readonly IUserRepository _userRepository;

    private readonly UserRoomObservable _userRoomObservable;

    public WebSocketController(IRoomRepository roomRepository, IUserRepository userRepository, UserRoomObservable userRoomObservable)
    {
        _roomRepository = roomRepository;
        _userRepository = userRepository;
        _userRoomObservable = userRoomObservable;
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

        var user = HttpContext.User?.ToUser();
        if (user == null)
        {
            HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
            return;
        }

        using var webSocket = await HttpContext.WebSockets.AcceptWebSocketAsync();

        try
        {
            var buffer = new byte[1024 * 4];
            var receiveResult = await webSocket.ReceiveAsync(
                new ArraySegment<byte>(buffer), CancellationToken.None);

            var userMessage = Encoding.ASCII.GetString(buffer, 0, receiveResult.Count);
            var roomRequest = JsonSerializer.Deserialize<RoomSubscribeRequest>(userMessage);

            var currentRoom = await _roomRepository.FindByIdAsync(roomRequest.RoomId);
            if (currentRoom == null)
            {
                await webSocket.CloseAsync(WebSocketCloseStatus.InvalidPayloadData, "Unknown room", CancellationToken.None);
                return;
            }

            if (!await _roomRepository.HasUserAsync(roomRequest.RoomId, user.Id))
            {
                var dbUser = await _userRepository.FindByIdAsync(user.Id);

                currentRoom.Users.Add(dbUser);

                await _roomRepository.UpdateAsync(currentRoom);
            }

            await _userRoomObservable.SubscribeAsync(currentRoom.Id, webSocket);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            await webSocket.CloseAsync(WebSocketCloseStatus.InvalidPayloadData, e.Message, CancellationToken.None);
        }
    }

    public class RoomSubscribeRequest
    {
        [JsonPropertyName("roomId")]
        public Guid RoomId { get; set; }
    }
}
