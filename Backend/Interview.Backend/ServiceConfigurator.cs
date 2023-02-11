using System.Net;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Text.Json;
using Interview.Backend.options;
using Interview.DependencyInjection;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.EntityFrameworkCore;

namespace Interview.Backend;

public class ServiceConfigurator
{
    private readonly IHostEnvironment _environment;
    private readonly IConfiguration _configuration;

    public ServiceConfigurator(IHostEnvironment environment, IConfiguration configuration)
    {
        _environment = environment;
        _configuration = configuration;
    }

    public void AddServices(IServiceCollection serviceCollection)
    {
        serviceCollection.AddControllers();
        
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        serviceCollection.AddEndpointsApiExplorer();
        serviceCollection.AddSwaggerGen();
        
        var serviceOption = new DependencyInjectionAppServiceOption(_configuration, optionsBuilder =>
        {
            if (_environment.IsDevelopment())
                optionsBuilder.UseSqlite(_configuration.GetConnectionString("sqlite"));
        });
        serviceCollection.AddAppServices(serviceOption);

        AddAuth(serviceCollection);
    }

    private void AddAuth(IServiceCollection serviceCollection)
    {
        var oAuthTwitchOptions = new OAuthTwitchOptions();
        _configuration.GetSection(OAuthTwitchOptions.OAuthTwitch).Bind(oAuthTwitchOptions);

        serviceCollection.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
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

                options.ClaimActions.MapJsonKey(ClaimTypes.Email, "email", "email");
                options.ClaimActions.MapJsonKey(ClaimTypes.Name, "display_name", "name");
                options.ClaimActions.MapJsonKey(ClaimTypes.NameIdentifier, "login", "nameIdentifier");
                options.ClaimActions.MapJsonKey(ClaimTypes.Hash, "id", "id");

                var scopes = new List<string>(oAuthTwitchOptions.Scopes);
                scopes.ForEach(scope => { options.Scope.Add(scope.ToLower()); });

                options.Events = new OAuthEvents()
                {
                    OnRemoteFailure = async ctx =>
                    {
                        if (ctx.Failure?.Message != null)
                        {
                            ctx.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                            await ctx.Response.WriteAsync(ctx.Failure.Message);
                        }

                        ctx.HandleResponse();
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

        serviceCollection.AddAuthorization(options =>
        {
            options.AddPolicy("user", policyBuilder =>
            {
                policyBuilder.AddAuthenticationSchemes("twitch")
                    .RequireAuthenticatedUser();
            });
        });
    }
}