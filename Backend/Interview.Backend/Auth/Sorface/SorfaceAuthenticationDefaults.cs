namespace Interview.Backend.Auth.Sorface;

public static class SorfaceAuthenticationDefaults
{
    public const string AuthenticationScheme = "Sorface";

    public static readonly string DisplayName = "Sorface";

    public static readonly string Issuer = "Sorface";

    public static readonly string CallbackPath = "/oauth2/sorface";

    public static readonly string AuthorizationEndPoint = "https://sso.sorface.com/oauth2/authorize";

    public static readonly string TokenEndpoint = "https://sso.sorface.com/oauth2/token";

    public static readonly string UserInformationEndpoint = "https://sso.sorface.com/oauth2/introspect";
}
