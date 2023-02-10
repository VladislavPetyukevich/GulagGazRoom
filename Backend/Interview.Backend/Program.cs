using System.Net.Http.Headers;
using System.Security.Claims;
using System.Security.Principal;
using System.Text.Json;
using Interview.Domain.Questions;
using Interview.Domain.Rooms;
using Interview.Domain.Users;
using Interview.Infrastructure;
using Interview.Infrastructure.Database;
using Interview.Infrastructure.Questions;
using Interview.Infrastructure.Rooms;
using Interview.Infrastructure.Users;
using Microsoft.AspNet.Authentication.OAuth;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Authentication.OAuth.Claims;
using Microsoft.AspNetCore.CookiePolicy;
using Microsoft.EntityFrameworkCore;
using TwitchLib.Api.Core.Enums;
using OAuthCreatingTicketContext = Microsoft.AspNetCore.Authentication.OAuth.OAuthCreatingTicketContext;
using OAuthEvents = Microsoft.AspNetCore.Authentication.OAuth.OAuthEvents;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var services = builder.Services;

services.AddScoped<IUserRepository, UserRepository>();
services.AddScoped<IRoomRepository, RoomRepository>();
services.AddScoped<IQuestionRepository, QuestionRepository>();

services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
services.AddEndpointsApiExplorer();
services.AddSwaggerGen();
services.AddDbContext<AppDbContext>(optionsBuilder =>
{
    if (builder.Environment.IsDevelopment())
        optionsBuilder.UseSqlite(builder.Configuration.GetConnectionString("sqlite"));
});

services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.Cookie.Name = "COMMUNIST";
        options.LoginPath = "/login";
        options.CookieManager = new ChunkingCookieManager();
    })
    .AddOAuth("twitch", options =>
    {
        options.SignInScheme = "Cookies";
        options.ClientId = "******************";
        options.ClientSecret = "********************";

        options.AuthorizationEndpoint = "https://id.twitch.tv/oauth2/authorize";
        options.CallbackPath = new PathString("/oauth2/callback");
        options.UsePkce = true;
        options.ClaimsIssuer = "twitch";
        options.SaveTokens = true;
        options.UsePkce = true;
        options.TokenEndpoint = "https://id.twitch.tv/oauth2/token";
        options.UserInformationEndpoint = "https://api.twitch.tv/helix/users";

        options.ClaimActions.MapJsonKey(ClaimTypes.Email, "email");
        options.ClaimActions.MapJsonKey(ClaimTypes.Name, "display_name");
        options.ClaimActions.MapJsonKey(ClaimTypes.NameIdentifier, "login");
        options.ClaimActions.MapJsonKey(ClaimTypes.Hash, "id");

        options.Scope.Add(AuthScopes.User_Read.ToString().ToLower());

        options.Events = new OAuthEvents()
        {
            OnTicketReceived = context =>
            {
                if (context.Principal != null)
                {
                    foreach (var claim  in context.Principal.Claims)
                    {
                        Console.WriteLine($"{claim.Type}-{claim.Value}");
                    }
                }

                return Task.CompletedTask;
            },

            OnCreatingTicket = async context =>
            {
                var request = new HttpRequestMessage(HttpMethod.Get, context.Options.UserInformationEndpoint);
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", context.AccessToken);
                request.Headers.Add("Client-Id", context.Options.ClientId);
                request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var response = await context.Backchannel.SendAsync(request, context.HttpContext.RequestAborted);
                response.EnsureSuccessStatusCode();

                using var user = JsonDocument.Parse(await response.Content.ReadAsStringAsync());

                context.RunClaimActions(user.RootElement.GetProperty("data")[0]);
            }
        };
    });

var app = builder.Build();

using (var serviceScope = app.Services.CreateScope())
{
    var appDbContext = serviceScope.ServiceProvider.GetRequiredService<AppDbContext>();
    appDbContext.Database.EnsureCreated();
}

app.UseCookiePolicy(new CookiePolicyOptions
{
    MinimumSameSitePolicy = SameSiteMode.Lax,
});
app.UseHttpsRedirection();

app.UseAuthorization();
app.UseAuthentication();

app.UseWebSockets();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();

app.Map("/login", signinApp =>
{
    signinApp.Run(async context =>
    {
        var authType = context.Request.Query["authscheme"];
        if (!string.IsNullOrEmpty(authType))
        {
            await context.ChallengeAsync(authType, new AuthenticationProperties() { RedirectUri = "/" });
            return;
        }

        var response = context.Response;
        response.ContentType = "text/html";
        await response.WriteAsync("<html><body>");
        await response.WriteAsync("Choose an authentication scheme: <br>");
        var schemeProvider = context.RequestServices.GetRequiredService<IAuthenticationSchemeProvider>();
        foreach (var provider in await schemeProvider.GetAllSchemesAsync())
        {
            await response.WriteAsync("<a href=\"?authscheme=" + provider.Name + "\">" +
                                      (provider.DisplayName ?? "(suppressed)") + "</a><br>");
        }

        await response.WriteAsync("</body></html>");
    });
});

app.Run();