using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using Interview.Backend.WebSocket.Events.ConnectionListener;

namespace Interview.Backend.WebSocket.Events.Handlers;

public class ReturningSignalWebSocketEventHandler : WebSocketEventHandlerBase<ReturningSignalWebSocketEventHandler.ReceivePayload>
{
    private readonly IVideChatConnectionProvider _userWebSocketConnectionProvider;

    public ReturningSignalWebSocketEventHandler(
        ILogger<WebSocketEventHandlerBase<ReceivePayload>> logger,
        IVideChatConnectionProvider userWebSocketConnectionProvider)
        : base(logger)
    {
        _userWebSocketConnectionProvider = userWebSocketConnectionProvider;
    }

    protected override string SupportType => "returning signal";

    protected override async Task HandleEventAsync(SocketEventDetail detail, ReceivePayload payload, CancellationToken cancellationToken)
    {
        if (!_userWebSocketConnectionProvider.TryGetConnections(payload.To, detail.RoomId, out var connections))
        {
            Logger.LogWarning("Not found {To} user connections. {RoomId} current {UserId}", payload.To, detail.RoomId, detail.UserId);
            return;
        }

        var receivingReturnedSignalPayload = new { Signal = payload.Signal, From = detail.UserId };
        var sendEvent = new WebSocketEvent
        {
            Type = "receiving returned signal",
            Payload = JsonSerializer.Serialize(receivingReturnedSignalPayload),
        };
        var sendEventAsStr = JsonSerializer.Serialize(sendEvent);
        var sendEventAsBytes = Encoding.UTF8.GetBytes(sendEventAsStr);
        foreach (var webSocket in connections)
        {
            try
            {
                await webSocket.SendAsync(sendEventAsBytes, WebSocketMessageType.Text, true, cancellationToken);
            }
            catch
            {
                // ignore
            }
        }
    }

    public sealed class ReceivePayload
    {
        public Guid To { get; set; }

        public string? Signal { get; set; }
    }
}
