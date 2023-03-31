using Interview.Backend.WebSocket.Configuration;
using Microsoft.AspNetCore.CookiePolicy;

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
        if (_app.Environment.IsPreProduction() || _app.Environment.IsProduction())
        {
            _app.UseHsts();
        }

        _app.UseHttpsRedirection();

        _app.UseCookiePolicy(new CookiePolicyOptions
        {
            MinimumSameSitePolicy = SameSiteMode.Lax, HttpOnly = HttpOnlyPolicy.None,
        });

        _app.UseWebSockets().UseWebSocketsAuthorization(new WebSocketAuthorizationOptions
        {
            CookieName = WebSocketAuthorizationOptions.DefaultCookieName, WebSocketQueryName = "Authorization",
        });

        if (_app.Environment.IsDevelopment())
        {
            _app.UseCors("All");
        }

        _app.UseRateLimiter();

        var logger = _app.Services.GetRequiredService<ILogger<MiddlewareConfigurator>>();

        _app.Use((context, func) =>
        {
            logger.LogInformation("Request {Path}", context.Request.Path);

            return func();
        });

        _app.UseAuthentication();
        _app.UseAuthorization();

        _app.UseSwagger();
        _app.UseSwaggerUI();

        _app.MapControllers();
    }
}
