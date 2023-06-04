using System.Buffers;
using System.Diagnostics.CodeAnalysis;
using System.Net.WebSockets;
using System.Text;
using CSharpFunctionalExtensions;
using Interview.Backend.Auth;
using Interview.Backend.WebSocket.UserByRoom;
using Interview.Domain.Connections;
using Interview.Domain.Events;
using Interview.Domain.Events.Events;
using Interview.Domain.RoomConfigurations;
using Interview.Domain.RoomParticipants;
using Interview.Domain.Rooms.Service;
using Microsoft.AspNetCore.Mvc;
using NSpecifications;

namespace Interview.Backend.WebSocket;

[ApiController]
[Route("[controller]")]
public class WebSocketController : ControllerBase
{
    private readonly RoomService _roomService;
    private readonly UserByRoomEventSubscriber _userByRoomEventSubscriber;
    private readonly IConnectUserSource _connectUserSource;

    public WebSocketController(UserByRoomEventSubscriber userByRoomEventSubscriber, RoomService roomService, IConnectUserSource connectUserSource)
    {
        _roomService = roomService;
        _connectUserSource = connectUserSource;
        _userByRoomEventSubscriber = userByRoomEventSubscriber;
    }

    [Route("/ws")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task ExecuteWebSocket()
    {
        if (!TryGetUser(out var user))
        {
            return;
        }

        var ct = HttpContext.RequestAborted;
        using var webSocket = await HttpContext.WebSockets.AcceptWebSocketAsync();

        (Guid RoomId, Guid UserId, string TwitchChannel)? connectionDetail = null;
        try
        {
            var roomIdentity = await ParseRoomIdAsync(webSocket, ct);
            if (roomIdentity is null)
            {
                return;
            }

            var (_, isFailure, dbRoom) = await _roomService.AddParticipantAsync(roomIdentity.Value, user.Id, ct);

            if (isFailure || dbRoom == null)
            {
                await webSocket.CloseAsync(WebSocketCloseStatus.InvalidPayloadData, "Invalid room details", ct);
                return;
            }

            var task = _userByRoomEventSubscriber.SubscribeAsync(dbRoom.Id, webSocket, ct);
            _connectUserSource.Connect(dbRoom.Id, user.Id, dbRoom.TwitchChannel);
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
        catch (OperationCanceledException)
        {
            // ignore
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
                _connectUserSource.Disconnect(roomId, userId, twitchChannel);
            }
        }
    }

    [Route("/code")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task SendCode()
    {
        if (!TryGetUser(out var user))
        {
            return;
        }

        var ct = HttpContext.RequestAborted;
        using var webSocket = await HttpContext.WebSockets.AcceptWebSocketAsync();

        try
        {
            var roomIdentity = await ParseRoomIdAsync(webSocket, ct);
            if (roomIdentity is null)
            {
                return;
            }

            var roomRepository = HttpContext.RequestServices.GetRequiredService<IRoomRepository>();
            var hasRoom = await roomRepository.HasAsync(new Spec<Room>(room => room.Id == roomIdentity), ct);
            if (!hasRoom)
            {
                await webSocket.CloseAsync(
                    WebSocketCloseStatus.InvalidPayloadData,
                    $"Not found room by id '{roomIdentity}'",
                    ct);
                return;
            }

            var participantRepository = HttpContext.RequestServices.GetRequiredService<IRoomParticipantRepository>();
            var roomParticipant = await participantRepository.FindByRoomIdAndUserId(roomIdentity.Value, user.Id, ct);
            if (roomParticipant is null)
            {
                await webSocket.CloseAsync(WebSocketCloseStatus.InvalidPayloadData, "Not found room participant", ct);
                return;
            }

            if (roomParticipant.Type != RoomParticipantType.Examinee &&
                roomParticipant.Type != RoomParticipantType.Expert)
            {
                await webSocket.CloseAsync(
                    WebSocketCloseStatus.InvalidPayloadData,
                    "Not enough rights to send an event.",
                    ct);
                return;
            }

            while (!webSocket.ShouldCloseWebSocket())
            {
                string code;
                using (var buffer = new PoolItem(8192))
                {
                    using var ms = new MemoryStream();
                    WebSocketReceiveResult result;
                    do
                    {
                        result = await webSocket.ReceiveAsync(buffer.Buffer, CancellationToken.None);
                        ms.Write(buffer.Buffer, 0, result.Count);
                    }
                    while (!result.EndOfMessage);

                    ms.Seek(0, SeekOrigin.Begin);

                    using var reader = new StreamReader(ms, Encoding.UTF8);
                    code = await reader.ReadToEndAsync(ct);
                }

                var repository = HttpContext.RequestServices.GetRequiredService<IRoomConfigurationRepository>();
                var request = new UpsertCodeStateRequest { CodeEditorContent = code, RoomId = roomIdentity.Value, };
                await repository.UpsertCodeStateAsync(request, ct);
            }

            try
            {
                await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, string.Empty, ct);
            }
            catch
            {
                // ignore
            }
        }
        catch (OperationCanceledException)
        {
            // ignore
        }
        catch (Exception e)
        {
            try
            {
                await webSocket.CloseAsync(WebSocketCloseStatus.InvalidPayloadData, e.Message, ct);
            }
            catch
            {
                // ignore
            }
        }
    }

    private async Task<Guid?> ParseRoomIdAsync(System.Net.WebSockets.WebSocket webSocket, CancellationToken ct)
    {
        if (!HttpContext.Request.Query.TryGetValue("roomId", out var roomIdentityString))
        {
            await webSocket.CloseAsync(WebSocketCloseStatus.InvalidPayloadData, "Invalid room details", ct);
            return null;
        }

        if (!Guid.TryParse(roomIdentityString, out var roomIdentity))
        {
            await webSocket.CloseAsync(WebSocketCloseStatus.InvalidPayloadData, "Invalid room details", ct);
            return null;
        }

        return roomIdentity;
    }

    private bool TryGetUser([NotNullWhen(true)] out User? user)
    {
        if (!HttpContext.WebSockets.IsWebSocketRequest)
        {
            HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
            user = null;
            return false;
        }

        user = HttpContext.User.ToUser();
        if (user == null)
        {
            HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
            return false;
        }

        return true;
    }

    private class PoolItem : IDisposable
    {
        public byte[] Buffer { get; }

        public PoolItem(int size)
        {
            Buffer = ArrayPool<byte>.Shared.Rent(size);
        }

        public void Dispose()
        {
            ArrayPool<byte>.Shared.Return(Buffer);
        }
    }
}
