using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Text.Json;
using Interview.Backend.options;
using Interview.Domain.Questions;
using Interview.Domain.Rooms;
using Interview.Domain.Users;
using Interview.Infrastructure.Database;
using Interview.Infrastructure.Questions;
using Interview.Infrastructure.Rooms;
using Interview.Infrastructure.Users;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using TwitchLib.Api.Core.Enums;
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

var oAuthTwitchOptions = new OAuthTwitchOptions();
builder.Configuration.GetSection(OAuthTwitchOptions.OAuthTwitch).Bind(oAuthTwitchOptions);

services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
    {
        options.Cookie.Name = "_communist";
        options.LoginPath = "/login";
        options.LogoutPath = "/logout";
        options.ClaimsIssuer = oAuthTwitchOptions.ClaimsIssuer;
        options.ExpireTimeSpan = TimeSpan.FromDays(10);
    })
    .AddOAuth("twitch", options =>
    {
        options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;

        options.ClientId = oAuthTwitchOptions.ClientId;
        options.ClientSecret = oAuthTwitchOptions.ClientSecret;

        options.ClaimsIssuer = oAuthTwitchOptions.ClaimsIssuer;

        options.UsePkce = oAuthTwitchOptions.UsePkce;
        options.SaveTokens = oAuthTwitchOptions.SaveTokens;

        options.CallbackPath = oAuthTwitchOptions.CallbackPath;
        options.AuthorizationEndpoint = oAuthTwitchOptions.AuthorizationEndpoint;
        options.TokenEndpoint = oAuthTwitchOptions.TokenEndpoint;
        options.UserInformationEndpoint = oAuthTwitchOptions.UserInformationEndpoint;

        options.ClaimActions.MapJsonKey(ClaimTypes.Email, "email", "data");
        options.ClaimActions.MapJsonKey(ClaimTypes.Name, "display_name", "data");
        options.ClaimActions.MapJsonKey(ClaimTypes.NameIdentifier, "login", "data");
        options.ClaimActions.MapJsonKey(ClaimTypes.Hash, "id", "data");

        var scopes = new List<string>(oAuthTwitchOptions.Scopes);
        scopes.ForEach(scope => { options.Scope.Add(scope.ToLower()); });

        options.Events = new OAuthEvents()
        {
            OnRemoteFailure = ctx =>
            {
                if (ctx.Failure?.Message != null)
                {
                    ctx.Response.Redirect("/error?FailureMessage=" +
                                          UrlEncoder.Default.Encode(ctx.Failure.Message));
                }

                ctx.HandleResponse();
                return Task.FromResult(0);
            },

            OnTicketReceived = context =>
            {
                if (context.Principal != null)
                {
                    foreach (var claim in context.Principal.Claims)
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

services.AddAuthorization(options =>
{
    options.AddPolicy("user", policyBuilder =>
    {
        policyBuilder.AddAuthenticationSchemes("twitch")
            .RequireAuthenticatedUser();
    });
});

var app = builder.Build();

app.MapGet("oauth2/login/twitch", async context =>
        await context.ChallengeAsync("twitch"))
    .RequireAuthorization("user");

using (var serviceScope = app.Services.CreateScope())
{
    var appDbContext = serviceScope.ServiceProvider.GetRequiredService<AppDbContext>();
    appDbContext.Database.EnsureCreated();
}

app.UseWebSockets();

app.UseCookiePolicy(new CookiePolicyOptions
{
    MinimumSameSitePolicy = SameSiteMode.Lax,
});
app.UseHttpsRedirection();

app.UseAuthorization();
app.UseAuthentication();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Choose an authentication type
app.Map("/login", signinApp =>
{
    signinApp.Run(async context =>
    {
        var authType = context.Request.Query["authscheme"];
        if (!string.IsNullOrEmpty(authType))
        {
            // By default the client will be redirect back to the URL that issued the challenge (/login?authtype=foo),
            // send them to the home page instead (/).
            await context.ChallengeAsync(authType, new AuthenticationProperties() { RedirectUri = "/" });
            return;
        }

        context.Response.ContentType = "text/html";
        await context.Response.WriteAsync("<html><body>");
        await context.Response.WriteAsync("Choose an authentication scheme: <br>");

        //foreach (var type in context.Authentication.GetAuthenticationSchemes())
        foreach (var type in context.RequestServices.GetRequiredService<IOptions<AuthenticationOptions>>().Value
                     .Schemes)
        {
            // TODO: display name?
            await context.Response.WriteAsync("<a href=\"?authscheme=" + type.Name + "\">" +
                                              (type.Name ?? "(suppressed)") + "</a><br>");
        }

        await context.Response.WriteAsync("</body></html>");
    });
});

// Sign-out to remove the user cookie.
app.Map("/logout", signoutApp =>
{
    signoutApp.Run(async context =>
    {
        context.Response.ContentType = "text/html";
        await context.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        await context.Response.WriteAsync("<html><body>");
        await context.Response.WriteAsync("You have been logged out. Goodbye " + context.User.Identity.Name + "<br>");
        await context.Response.WriteAsync("<a href=\"/\">Home</a>");
        await context.Response.WriteAsync("</body></html>");
    });
});

// Display the remote error
app.Map("/error", errorApp =>
{
    errorApp.Run(async context =>
    {
        context.Response.ContentType = "text/html";
        await context.Response.WriteAsync("<html><body>");
        await context.Response.WriteAsync("An remote failure has occurred: " + context.Request.Query["FailureMessage"] +
                                          "<br>");
        await context.Response.WriteAsync("<a href=\"/\">Home</a>");
        await context.Response.WriteAsync("</body></html>");
    });
});

app.MapControllers();


app.Run();