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
        
        _app.Use(((context, next) =>
        {
            if (context.WebSockets.IsWebSocketRequest)
            {
                var headers = context.Request.Headers;
                if (!headers.TryGetValue("Authorization", out var value))
                {
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    return Task.CompletedTask;
                }

                context.Request.Headers["Cookie"] = $"_communist={value}";
                context.Request.Cookies = new ReqCollection { { "_communist", value } };
            }
            
            return next();
        }));
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
    
    private sealed class ReqCollection : Dictionary<string, string>, IRequestCookieCollection
    {
        public ICollection<string> Keys => this.Select(e => e.Key).ToList();
    }
}
