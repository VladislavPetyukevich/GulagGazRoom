using Interview.Backend.WebSocket.Configuration;

namespace Interview.Backend;

public class MiddlewareConfigurator
{
    private readonly WebApplication _app;

    public MiddlewareConfigurator(WebApplication app)
    {
        _app = app;
    }

    public void AddMiddlewares()
    {
        _app.UseHttpsRedirection();

        _app.UseCookiePolicy(new CookiePolicyOptions { MinimumSameSitePolicy = SameSiteMode.Lax, });

        _app.UseWebSockets()
            .UseWebSocketsAuthorization(new WebSocketAuthorizationOptions
            {
                CookieName = WebSocketAuthorizationOptions.DefaultCookieName,
                WebSocketHeaderName = "Authorization",
            });

        if (_app.Environment.IsDevelopment())
        {
            _app.UseCors("All");
        }

        _app.UseAuthentication();
        _app.UseAuthorization();

        _app.UseSwagger();
        _app.UseSwaggerUI();

        _app.MapControllers();
    }
}
