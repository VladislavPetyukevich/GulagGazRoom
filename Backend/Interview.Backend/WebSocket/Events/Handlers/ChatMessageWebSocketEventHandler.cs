using Interview.Domain.Events;
using Interview.Domain.Events.Events;
using Interview.Infrastructure.Chat;

namespace Interview.Backend.WebSocket.Events.Handlers;

public class ChatMessageWebSocketEventHandler : WebSocketEventHandlerBase
{
    private readonly IRoomEventDispatcher _eventDispatcher;

    public ChatMessageWebSocketEventHandler(
        IRoomEventDispatcher eventDispatcher,
        ILogger<WebSocketEventHandlerBase> logger)
        : base(logger)
    {
        _eventDispatcher = eventDispatcher;
    }

    protected override string SupportType => "chat-message";

    protected override Task HandleEventAsync(SocketEventDetail detail, string message, CancellationToken cancellationToken)
    {
        var payload = new UserMessageEventPayload(message, detail.User.Nickname);
        var @event = new RoomEvent<UserMessageEventPayload>(detail.RoomId, EventType.ChatMessage, payload);
        return _eventDispatcher.WriteAsync(@event, cancellationToken);
    }
}
