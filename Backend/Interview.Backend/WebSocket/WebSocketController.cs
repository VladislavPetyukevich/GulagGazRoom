using System.Buffers;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Interview.Backend.Auth;
using Interview.Backend.WebSocket.UserByRoom;
using Microsoft.AspNetCore.Mvc;

namespace Interview.Backend.WebSocket;

[ApiController]
[Route("[controller]")]
public class WebSocketController : ControllerBase
{
    private readonly IRoomRepository _roomRepository;
    private readonly IUserRepository _userRepository;

    private readonly UserByRoomEventSubscriber _userByRoomEventSubscriber;

    public WebSocketController(IRoomRepository roomRepository, IUserRepository userRepository, UserByRoomEventSubscriber userByRoomEventSubscriber)
    {
        _roomRepository = roomRepository;
        _userRepository = userRepository;
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

        var user = HttpContext.User?.ToUser();
        if (user == null)
        {
            HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
            return;
        }

        using var webSocket = await HttpContext.WebSockets.AcceptWebSocketAsync();

        var pool = ArrayPool<byte>.Shared;
        var buffer = pool.Rent(1024 * 4);

        try
        {
            var receiveResult = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

            var userMessage = Encoding.ASCII.GetString(buffer, 0, receiveResult.Count);
            var roomRequest = JsonSerializer.Deserialize<RoomSubscribeRequest>(userMessage);
            if (roomRequest == null)
            {
                await webSocket.CloseAsync(WebSocketCloseStatus.InvalidPayloadData, "Invalid room details", CancellationToken.None);
                return;
            }

            var currentRoom = await PrepareRoomAsync(roomRequest, webSocket, user);
            if (currentRoom == null)
            {
                return;
            }

            await _userByRoomEventSubscriber.SubscribeAsync(currentRoom.Id, webSocket);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            await webSocket.CloseAsync(WebSocketCloseStatus.InvalidPayloadData, e.Message, CancellationToken.None);
        }
        finally
        {
            pool.Return(buffer);
        }
    }

    private async Task<Room?> PrepareRoomAsync(RoomSubscribeRequest roomRequest, System.Net.WebSockets.WebSocket webSocket, User user)
    {
        var currentRoom = await _roomRepository.FindByIdAsync(roomRequest.RoomId);
        if (currentRoom == null)
        {
            await webSocket.CloseAsync(WebSocketCloseStatus.InvalidPayloadData, "Unknown room", CancellationToken.None);
            return null;
        }

        if (await _roomRepository.HasUserAsync(roomRequest.RoomId, user.Id))
        {
            return currentRoom;
        }

        var dbUser = await _userRepository.FindByIdAsync(user.Id);
        if (dbUser == null)
        {
            await webSocket.CloseAsync(WebSocketCloseStatus.InvalidPayloadData, "Unknown user", CancellationToken.None);
            return null;
        }

        currentRoom.Users.Add(dbUser);
        await _roomRepository.UpdateAsync(currentRoom);
        return currentRoom;
    }

    public class RoomSubscribeRequest
    {
        [JsonPropertyName("roomId")]
        public Guid RoomId { get; set; }
    }
}
