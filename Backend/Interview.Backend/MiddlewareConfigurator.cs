using System.Net.Http.Headers;
using Interview.DependencyInjection;
using Microsoft.AspNetCore.Http.Headers;

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

        _app.UseCookiePolicy(new CookiePolicyOptions
        {
            MinimumSameSitePolicy = SameSiteMode.Lax,
        });

        _app.UseWebSockets();

        _app.UseWebSocketsAuthorization(new WebSocketAuthorizationOptions()
        {
            CookieName = "_communist",

            WebSocketHeaderName = "Authorization",
        });

        _app.UseAuthentication();
        _app.UseAuthorization();

        // Configure the HTTP request pipeline.
        if (_app.Environment.IsDevelopment())
        {
            _app.UseSwagger();
            _app.UseSwaggerUI();
        }

        _app.MapControllers();
    }
}
