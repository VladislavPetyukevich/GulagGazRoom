using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using Interview.Backend.WebSocket.Events.ConnectionListener;

namespace Interview.Backend.WebSocket.Events.Handlers;

public class SendingSignalWebSocketEventHandler : WebSocketEventHandlerBase<SendingSignalWebSocketEventHandler.ReceivePayload>
{
    private readonly IUserWebSocketConnectionProvider _userWebSocketConnectionProvider;

    public SendingSignalWebSocketEventHandler(
        ILogger<WebSocketEventHandlerBase<ReceivePayload>> logger,
        IUserWebSocketConnectionProvider userWebSocketConnectionProvider)
        : base(logger)
    {
        _userWebSocketConnectionProvider = userWebSocketConnectionProvider;
    }

    protected override string SupportType => "sending signal";

    protected override async Task HandleEventAsync(SocketEventDetail detail, ReceivePayload payload, CancellationToken cancellationToken)
    {
        if (!_userWebSocketConnectionProvider.TryGetConnections(payload.To, detail.RoomId, out var connections))
        {
            Logger.LogWarning("Not found {To} user connections. {RoomId} {From}", payload.To, detail.RoomId, detail.UserId);
            return;
        }

        var sendEvent = new WebSocketEvent
        {
            Type = "user joined",
            Payload = JsonSerializer.Serialize(new { From = detail.UserId, Signal = payload.Signal, }),
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
