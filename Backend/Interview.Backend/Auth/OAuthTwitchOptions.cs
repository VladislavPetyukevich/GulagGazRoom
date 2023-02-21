namespace Interview.Backend.Auth;

public class OAuthTwitchOptions
{
    public const string OAuthTwitch = "OAuthTwitch";

    public OAuthTwitchOptions(IConfiguration configuration)
    {
        configuration.GetSection(OAuthTwitch).Bind(this);
    }

    public string ClientId { get; set; } = string.Empty;

    public string ClientSecret { get; set; } = string.Empty;

    public string ClaimsIssuer { get; set; } = string.Empty;

    public bool UsePkce { get; set; } = false;

    public PathString CallbackPath { get; set; } = PathString.Empty;

    private OAuthTwitchOptions()
    {
    }

    public TwitchTokenProviderOption ToTwitchTokenProviderOption()
    {
        return new TwitchTokenProviderOption
        {
            ClientId = ClientId,
            ClientSecret = ClientSecret,
        };
    }
}
