namespace Interview.Backend.WebSocket.Events.Handlers
{
    public class JoinVideoChatWebSocketEventHandler : WebSocketEventHandlerBase
    {
        public JoinVideoChatWebSocketEventHandler(ILogger<JoinVideoChatWebSocketEventHandler> logger)
            : base(logger)
        {
        }

        protected override string SupportType => "join video chat";

        protected override Task HandleEventAsync(
            System.Net.WebSockets.WebSocket webSocket,
            WebSocketEvent @event,
            string payload,
            CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
