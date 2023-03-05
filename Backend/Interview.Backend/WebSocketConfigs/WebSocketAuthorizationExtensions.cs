using Interview.Backend.WebSockerConfigs;
using Microsoft.Extensions.Options;

namespace Interview.DependencyInjection
{
    public static class WebSocketAuthorizationExtensions
    {
        public static IApplicationBuilder UseWebSocketsAuthorization(this IApplicationBuilder app, WebSocketAuthorizationOptions options)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            if (app == null)
            {
                throw new ArgumentNullException(nameof(app));
            }

            return app.UseMiddleware<WebSocketAuthorizationMiddleware>(Options.Create(options));
        }
    }
}
