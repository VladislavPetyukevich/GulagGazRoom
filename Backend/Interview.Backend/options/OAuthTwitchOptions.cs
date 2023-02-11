namespace Interview.Backend.options;

public class OAuthTwitchOptions
{
    public const string OAuthTwitch = "OAuthTwitch";

    public string ClientId { get; set; } = string.Empty;

    public string ClientSecret { get; set; } = string.Empty;

    public string ClaimsIssuer { get; set; } = string.Empty;

    public bool UsePkce { get; set; } = false;
    
    public bool SaveTokens { get; set; } = false;

    public PathString CallbackPath { get; set; } = PathString.Empty;

    public string AuthorizationEndpoint { get; set; } = string.Empty;

    public string TokenEndpoint { get; set; } = string.Empty;

    public string UserInformationEndpoint { get; set; } = string.Empty;

    public string[] Scopes { get; set; } = Array.Empty<string>();

    public string[] Claims { get; set; } = Array.Empty<string>();
}