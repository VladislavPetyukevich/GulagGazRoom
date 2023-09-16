using System.Text.Json;

namespace Interview.Backend.WebSocket.Events.Handlers;

public abstract class WebSocketEventHandlerBase<TPayload> : IWebSocketEventHandler
{
    private readonly ILogger<WebSocketEventHandlerBase<TPayload>> _logger;

    protected WebSocketEventHandlerBase(ILogger<WebSocketEventHandlerBase<TPayload>> logger)
    {
        _logger = logger;
    }

    protected abstract string SupportType { get; }

    public Task HandleAsync(System.Net.WebSockets.WebSocket webSocket, WebSocketEvent @event, CancellationToken cancellationToken)
    {
        if (!SupportType.Equals(@event.Type, StringComparison.InvariantCultureIgnoreCase))
        {
            return Task.CompletedTask;
        }

        try
        {
            var payload = ParsePayload(@event);
            if (payload is not null)
            {
                return HandleEventAsync(webSocket, @event, payload, cancellationToken);
            }
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Unable to parse payload {Payload}", @event.Payload);
        }

        return Task.CompletedTask;
    }

    protected abstract Task HandleEventAsync(System.Net.WebSockets.WebSocket webSocket, WebSocketEvent @event, TPayload payload, CancellationToken cancellationToken);

    protected virtual TPayload? ParsePayload(WebSocketEvent @event) => JsonSerializer.Deserialize<TPayload>(@event.Payload);
}

public abstract class WebSocketEventHandlerBase : WebSocketEventHandlerBase<string>
{
    protected WebSocketEventHandlerBase(ILogger<WebSocketEventHandlerBase> logger)
        : base(logger)
    {
    }

    protected override string? ParsePayload(WebSocketEvent @event) => @event.Payload;
}
