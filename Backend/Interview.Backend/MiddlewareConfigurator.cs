using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Options;

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
        
        _app.UseAuthentication();
        _app.UseAuthorization();

        _app.UseWebSockets();

        // Configure the HTTP request pipeline.
        if (_app.Environment.IsDevelopment())
        {
            _app.UseSwagger();
            _app.UseSwaggerUI();
        }
        
        _app.MapControllers();
    }
}