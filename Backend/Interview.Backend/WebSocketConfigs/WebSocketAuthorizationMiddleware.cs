using Interview.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Interview.Backend.WebSockerConfigs
{
    public class WebSocketAuthorizationMiddleware
    {
        private readonly RequestDelegate _next;

        private readonly WebSocketAuthorizationOptions _options;

        public WebSocketAuthorizationMiddleware(RequestDelegate next, IOptions<WebSocketAuthorizationOptions> options)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            _next = next ?? throw new ArgumentNullException(nameof(next));
            _options = options.Value;
        }

        public Task Invoke(HttpContext context)
        {
            if (!context.WebSockets.IsWebSocketRequest)
            {
                return _next(context);
            }

            var headers = context.Request.Headers;

            if (!headers.TryGetValue(_options.WebSocketHeaderName, out var value))
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                return Task.CompletedTask;
            }

            context.Request.Headers.Cookie = $"{_options.CookieName}={value}";
            context.Request.Cookies = new ReqCollection { { $"{_options.CookieName}", value } };

            return _next(context);
        }

        private sealed class ReqCollection : Dictionary<string, string>, IRequestCookieCollection
        {
            public ICollection<string> Keys => this.Select(e => e.Key).ToList();
        }
    }
}
