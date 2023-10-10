using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using Interview.Backend.WebSocket.Events.ConnectionListener;

namespace Interview.Backend.WebSocket.Events.Handlers;

public class SendingSignalWebSocketEventHandler : WebSocketEventHandlerBase<SendingSignalWebSocketEventHandler.ReceivePayload>
{
    private readonly IVideChatConnectionProvider _userWebSocketConnectionProvider;

    public SendingSignalWebSocketEventHandler(
        ILogger<WebSocketEventHandlerBase<ReceivePayload>> logger,
        IVideChatConnectionProvider userWebSocketConnectionProvider)
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

        var payloadForSerialization = new UserDetailResponse
        {
            From = new UserDetail
            {
                Id = detail.User.Id,
                Nickname = detail.User.Nickname,
                Avatar = detail.User.Avatar,
            },
            Signal = payload.Signal,
        };
        var sendEvent = new WebSocketEvent
        {
            Type = "user joined",
            Payload = JsonSerializer.Serialize(payloadForSerialization),
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

public class UserDetailResponse
{
    public required UserDetail From { get; init; }

    public required string? Signal { get; init; }
}

public class UserDetail
{
    public required Guid Id { get; init; }

    public required string Nickname { get; init; }

    public required string? Avatar { get; init; }
}
