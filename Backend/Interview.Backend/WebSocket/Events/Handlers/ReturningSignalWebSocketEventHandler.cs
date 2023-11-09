using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using Interview.Backend.WebSocket.Events.ConnectionListener;
using Interview.Domain.Events.Events.Serializers;
using Interview.Domain.Events.Sender;

namespace Interview.Backend.WebSocket.Events.Handlers;

public class ReturningSignalWebSocketEventHandler : WebSocketEventHandlerBase<ReturningSignalWebSocketEventHandler.ReceivePayload>
{
    private readonly IVideChatConnectionProvider _userWebSocketConnectionProvider;
    private readonly ILogger<WebSocketEventSender> _webSocketEventSender;
    private readonly IEventSenderAdapter _eventSenderAdapter;
    private readonly IRoomEventSerializer _serializer;

    public ReturningSignalWebSocketEventHandler(
        ILogger<WebSocketEventHandlerBase<ReceivePayload>> logger,
        IVideChatConnectionProvider userWebSocketConnectionProvider,
        ILogger<WebSocketEventSender> webSocketEventSender,
        IEventSenderAdapter eventSenderAdapter,
        IRoomEventSerializer serializer)
        : base(logger)
    {
        _userWebSocketConnectionProvider = userWebSocketConnectionProvider;
        _webSocketEventSender = webSocketEventSender;
        _eventSenderAdapter = eventSenderAdapter;
        _serializer = serializer;
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
        var strPayload = JsonSerializer.Serialize(receivingReturnedSignalPayload);
        var sendEvent = new WebSocketEvent(detail.RoomId, "receiving returned signal", strPayload, false);
        var provider = new CachedRoomEventProvider(sendEvent, _serializer);
        foreach (var webSocket in connections)
        {
            var sender = new WebSocketEventSender(_webSocketEventSender, webSocket);
            await _eventSenderAdapter.SendAsync(provider, sender, cancellationToken);
        }
    }

    public sealed class ReceivePayload
    {
        public Guid To { get; set; }

        public string? Signal { get; set; }
    }
}
