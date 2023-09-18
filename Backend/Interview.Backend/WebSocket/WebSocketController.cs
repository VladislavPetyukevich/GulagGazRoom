using System.Buffers;
using System.Diagnostics.CodeAnalysis;
using System.Net.WebSockets;
using System.Text;
using Interview.Backend.Auth;
using Interview.Backend.WebSocket.Events;
using Interview.Backend.WebSocket.Events.ConnectionListener;
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
    private readonly ILogger<WebSocketController> _logger;
    private readonly IRoomService _roomService;
    private readonly IConnectionListener[] _connectListeners;
    private readonly WebSocketReader _webSocketReader;

    public WebSocketController(
        IRoomService roomService,
        WebSocketReader webSocketReader,
        IEnumerable<IConnectionListener> connectionListeners,
        ILogger<WebSocketController> logger)
    {
        _roomService = roomService;
        _connectListeners = connectionListeners.ToArray();
        _webSocketReader = webSocketReader;
        _logger = logger;
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
        WebSocketConnectDetail? detail = null;
        try
        {
            var roomIdentity = await ParseRoomIdAsync(webSocket, ct);
            if (roomIdentity is null)
            {
                return;
            }

            var dbRoom = await _roomService.AddParticipantAsync(roomIdentity.Value, user.Id, ct);

            detail = new WebSocketConnectDetail(webSocket, dbRoom, user);
            await HandleListenersSafely(
                nameof(IConnectionListener.OnConnectAsync),
                e => e.OnConnectAsync(detail, ct));

            var waitTask = CreateWaitTask(ct);
            var readerTask = RunEventReaderJob(user.Id, roomIdentity.Value, webSocket, ct);
            await Task.WhenAny(waitTask, readerTask);
            await CloseSafely(webSocket, WebSocketCloseStatus.NormalClosure, string.Empty, ct);
        }
        catch (OperationCanceledException)
        {
            // ignore
        }
        catch (Exception e)
        {
            await CloseSafely(webSocket, WebSocketCloseStatus.InvalidPayloadData, e.Message, ct);
        }
        finally
        {
            if (detail is not null)
            {
                await HandleListenersSafely(
                    nameof(IConnectionListener.OnDisconnectAsync),
                    e => e.OnDisconnectAsync(detail, ct));
            }
        }

        return;

        static async Task CreateWaitTask(CancellationToken cancellationToken)
        {
            var cst = new TaskCompletionSource<object>();
            await using (cancellationToken.Register(() => cst.TrySetCanceled()))
            {
                await cst.Task;
            }
        }

        static async Task CloseSafely(
            System.Net.WebSockets.WebSocket ws,
            WebSocketCloseStatus status,
            string message,
            CancellationToken cancellationToken)
        {
            try
            {
                await ws.CloseAsync(status, message, cancellationToken);
            }
            catch
            {
                // ignore
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

                if (webSocket.ShouldCloseWebSocket())
                {
                    break;
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

    private async Task HandleListenersSafely(string actionName, Func<IConnectionListener, Task> map)
    {
        var tasks = _connectListeners.Select(map);
        try
        {
            await Task.WhenAll(tasks);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "During {Action}", actionName);
        }
    }

    private Task RunEventReaderJob(Guid userId, Guid roomId, System.Net.WebSockets.WebSocket webSocket, CancellationToken ct)
    {
        return Task.Run(
            () =>
            {
                return _webSocketReader.ReadAsync(
                    userId,
                    roomId,
                    webSocket,
                    exception => Console.WriteLine("On error: {0}", exception),
                    ct);
            },
            ct);
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
