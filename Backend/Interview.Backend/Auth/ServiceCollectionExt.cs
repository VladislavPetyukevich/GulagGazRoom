using Interview.Backend.WebSocket.Configuration;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.DataProtection;

namespace Interview.Backend.Auth;

public static class ServiceCollectionExt
{
    public static void AddAppAuth(this IServiceCollection self, AuthorizationService authorizationService)
    {
        const string AuthenticationScheme = CookieAuthenticationDefaults.AuthenticationScheme;

        self.AddAuthentication(AuthenticationScheme)
            .AddCookie(AuthenticationScheme, options =>
            {
                options.Events.OnRedirectToAccessDenied = context =>
                {
                    context.Response.StatusCode = 403;
                    return Task.CompletedTask;
                };
                options.Events.OnRedirectToLogin = context =>
                {
                    context.Response.StatusCode = 401;
                    return Task.CompletedTask;
                };
                options.Cookie.HttpOnly = false;
                options.Cookie.Name = WebSocketAuthorizationOptions.DefaultCookieName;
                options.ClaimsIssuer = authorizationService.ClaimsIssuer;
                options.ExpireTimeSpan = TimeSpan.FromDays(10);
            })
            .AddTwitch(authorizationService.Id, options =>
            {
                options.ClientId = authorizationService.ClientId;
                options.ClientSecret = authorizationService.ClientSecret;
                options.CallbackPath = authorizationService.CallbackPath;
                options.ClaimsIssuer = authorizationService.ClaimsIssuer;
                options.Events.OnTicketReceived += async context =>
                {
                    var user = context.Principal?.ToUser();
                    if (user == null)
                    {
                        return;
                    }

                    var userService = context.HttpContext.RequestServices.GetRequiredService<UserService>();
                    var upsertUser = await userService.UpsertByTwitchIdentityAsync(user);
                    context.Principal!.EnrichRolesWithId(upsertUser);
                };
                options.Scope.Clear();
            });

        self.AddAuthorization(options =>
        {
            options.AddPolicy(GulagSecurePolicy.Manager, policyBuilder =>
            {
                policyBuilder.RequireRole(RoleNameConstants.Admin);
            });
            options.AddPolicy(GulagSecurePolicy.User, policyBuilder =>
            {
                policyBuilder.RequireAuthenticatedUser();
            });
        });
    }
}
