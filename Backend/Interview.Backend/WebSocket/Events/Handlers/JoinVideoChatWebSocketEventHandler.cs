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
        var successConnectResult = await _videChatConnectionProvider.TryConnectAsync(detail, cancellationToken);
        if (!successConnectResult)
        {
            return;
        }

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
        if (!_videChatConnectionProvider.TryGetConnections(detail.RoomId, out var subscribers))
        {
            return;
        }

        var users = subscribers
            .DistinctBy(e => e.User.Id)
            .Select(e => new UserDetail
            {
                Id = e.User.Id,
                Nickname = e.User.Nickname,
                Avatar = e.User.Avatar,
            }).ToList();
        var newEvent = new WebSocketEvent
        {
            Type = "all users",
            Payload = JsonSerializer.Serialize(users),
        };
        var eventAsStr = JsonSerializer.Serialize(newEvent);
        var bytes = Encoding.UTF8.GetBytes(eventAsStr);
        await detail.WebSocket.SendAsync(bytes, WebSocketMessageType.Text, true, cancellationToken);
    }
}
