using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using Interview.Backend.WebSocket.Events.ConnectionListener;

namespace Interview.Backend.WebSocket.Events.Handlers;

public class JoinVideoChatWebSocketEventHandler : WebSocketEventHandlerBase
{
    private readonly IVideChatConnectionProvider _videChatConnectionProvider;

    public JoinVideoChatWebSocketEventHandler(
        ILogger<JoinVideoChatWebSocketEventHandler> logger,
        IVideChatConnectionProvider videChatConnectionProvider)
        : base(logger)
    {
        _videChatConnectionProvider = videChatConnectionProvider;
    }

    protected override string SupportType => "join video chat";

    protected override async Task HandleEventAsync(SocketEventDetail detail, string payload, CancellationToken cancellationToken)
    {
        _videChatConnectionProvider.Connect(detail.Event.RoomId, detail.UserId, detail.WebSocket);

        try
        {
            await SendEventsAsync(detail, cancellationToken);
        }
        catch (Exception e)
        {
            Logger.LogError(e, "During send join video chat event");
        }
    }

    private async Task SendEventsAsync(SocketEventDetail detail, CancellationToken cancellationToken)
    {
        if (!_videChatConnectionProvider.TryGetConnections(detail.Event.RoomId, out var subscribers))
        {
            return;
        }

        var users = subscribers.Select(e => e.UserId).ToHashSet();
        var newEvent = new WebSocketEvent
        {
            RoomId = detail.Event.RoomId,
            Type = "all users",
            Payload = JsonSerializer.Serialize(users),
        };
        var eventAsStr = JsonSerializer.Serialize(newEvent);
        var bytes = Encoding.UTF8.GetBytes(eventAsStr);
        await detail.WebSocket.SendAsync(bytes, WebSocketMessageType.Text, true, cancellationToken);
    }
}
