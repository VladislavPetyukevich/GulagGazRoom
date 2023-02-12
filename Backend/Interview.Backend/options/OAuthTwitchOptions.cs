namespace Interview.Backend.options;

public class OAuthTwitchOptions
{
    public const string OAuthTwitch = "OAuthTwitch";

    public string ClientId { get; set; } = string.Empty;

    public string ClientSecret { get; set; } = string.Empty;

    public string ClaimsIssuer { get; set; } = string.Empty;

    public bool UsePkce { get; set; } = false;
    
    public PathString CallbackPath { get; set; } = PathString.Empty;
}