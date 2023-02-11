using System.Security.Claims;
using Ardalis.SmartEnum.SystemTextJson;
using Interview.Backend.options;
using Interview.DependencyInjection;
using Interview.Domain.Users;
using Interview.Domain.Users.Roles;
using Microsoft.AspNetCore.Authentication.Cookies;
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
        serviceCollection
            .AddControllers()
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.Converters.Add(new SmartEnumNameConverter<RoleName, int>());
            });
        
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
            .AddTwitch("twitch", options =>
            {
                options.ClientId = oAuthTwitchOptions.ClientId;
                options.ClientSecret = oAuthTwitchOptions.ClientSecret;
                options.CallbackPath = oAuthTwitchOptions.CallbackPath;
                options.ClaimsIssuer = oAuthTwitchOptions.ClaimsIssuer;
                options.UsePkce = oAuthTwitchOptions.UsePkce;
                options.Events.OnTicketReceived += async context =>
                {
                    if(context.Principal == null)
                        return;

                    var user = ToUser(context.Principal);
                    if(user == null)
                        return;
                    
                    var userService = context.HttpContext.RequestServices.GetRequiredService<UserService>();
                    await userService.CreateOrUpdateAsync(user);
                };
            });

        serviceCollection.AddAuthorization(options =>
        {
            options.AddPolicy("user", policyBuilder =>
            {
                policyBuilder.AddAuthenticationSchemes("twitch").RequireAuthenticatedUser();
            });
        });
    }

    private static User? ToUser(ClaimsPrincipal claimsPrincipal)
    {
        var id = claimsPrincipal.Claims.FirstOrDefault(e => e.Type == ClaimTypes.NameIdentifier);
        var email = claimsPrincipal.Claims.FirstOrDefault(e => e.Type == ClaimTypes.Email);
        var nickname = claimsPrincipal.Claims.FirstOrDefault(e => e.Type == ClaimTypes.Name);
        if(id == null || email == null || nickname == null)
            return null;
                    
        return new User(nickname.Value, email.Value, id.Value);
    }
}