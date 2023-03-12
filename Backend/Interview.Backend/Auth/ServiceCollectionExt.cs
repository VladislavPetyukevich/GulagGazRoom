using Interview.Backend.WebSocket.Configuration;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace Interview.Backend.Auth;

public static class ServiceCollectionExt
{
    public static void AddAppAuth(this IServiceCollection self, OAuthTwitchOptions oAuthTwitchOptions)
    {
        const string TwitchScheme = "twitch";
        var authenticationScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        self.AddAuthentication(authenticationScheme)
            .AddCookie(authenticationScheme, options =>
            {
                options.Cookie.HttpOnly = false;
                options.Cookie.Name = WebSocketAuthorizationOptions.DefaultCookieName;
                options.LoginPath = "/login";
                options.LogoutPath = "/logout";
                options.ClaimsIssuer = oAuthTwitchOptions.ClaimsIssuer;
                options.ExpireTimeSpan = TimeSpan.FromDays(10);
            })
            .AddTwitch(TwitchScheme, options =>
            {
                options.ClientId = oAuthTwitchOptions.ClientId;
                options.ClientSecret = oAuthTwitchOptions.ClientSecret;
                options.CallbackPath = oAuthTwitchOptions.CallbackPath;
                options.ClaimsIssuer = oAuthTwitchOptions.ClaimsIssuer;
                options.UsePkce = oAuthTwitchOptions.UsePkce;
                options.Events.OnAccessDenied += context =>
                {
                    Console.WriteLine(context);

                    return Task.CompletedTask;
                };
                options.Events.OnAccessDenied += context =>
                {
                    context.HttpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;

                    return Task.CompletedTask;
                };
                options.Events.OnTicketReceived += async context =>
                {
                    var user = context.Principal?.ToUser();
                    if (user == null)
                    {
                        return;
                    }

                    var userService = context.HttpContext.RequestServices.GetRequiredService<UserService>();
                    await userService.UpsertByTwitchIdentityAsync(user);
                    context.Principal!.EnrichRolesWithId(user);
                };
            });

        self.AddAuthorization(options =>
        {
            options.AddPolicy(OAuthTwitchOptions.Policy, policyBuilder =>
            {
                policyBuilder.AddAuthenticationSchemes(TwitchScheme).RequireAuthenticatedUser();
            });
        });
    }
}
