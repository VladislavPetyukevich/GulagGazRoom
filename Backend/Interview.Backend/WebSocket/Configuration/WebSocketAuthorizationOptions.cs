namespace Interview.Backend.WebSocket.Configuration;

public class WebSocketAuthorizationOptions
{
    public const string DefaultCookieName = "_communist";

    public string CookieName { get; set; } = string.Empty;

    public string WebSocketHeaderName { get; set; } = string.Empty;
}
