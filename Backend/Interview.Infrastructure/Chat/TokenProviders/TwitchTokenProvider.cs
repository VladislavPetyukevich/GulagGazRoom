using System.Text;
using System.Text.Json;

namespace Interview.Infrastructure.Chat.TokenProviders;

public sealed class TwitchTokenProvider : ITwitchTokenProvider
{
    private const string TwitchOAuth2Uri = "https://id.twitch.tv/oauth2/";
    private const string DefaultScope = "chat:edit chat:read";
    private const string DefaultGrantType = "client_credentials";

    private readonly TwitchTokenProviderOption _option;

    public TwitchTokenProvider(TwitchTokenProviderOption options)
    {
        _option = options;
    }

    public async ValueTask<TwitchToken> GetTokenAsync(CancellationToken cancellationToken = default)
    {
        using var httpClient = new HttpClient { BaseAddress = new Uri(TwitchOAuth2Uri) };
        using var request = new HttpRequestMessage(HttpMethod.Post, "token") { Content = BuildStringContent() };
        using var response = await httpClient.SendAsync(request, cancellationToken);
        var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
        return ExtractAccessToken(responseContent);
    }

    private static TwitchToken ExtractAccessToken(string jsonResponse)
    {
        var json = JsonDocument.Parse(jsonResponse);
        var accessTokenProperty = json.RootElement.GetProperty("access_token");
        var expiresIn = json.RootElement.GetProperty("expires_in");
        return new TwitchToken(accessTokenProperty.GetString(), expiresIn.GetInt64());
    }

    private StringContent BuildStringContent()
    {
        var content = "client_id=" + _option.ClientId +
                      "&client_secret=" + _option.ClientSecret +
                      "&grant_type=" + DefaultGrantType +
                      "&scope=" + DefaultScope;
        return new StringContent(content, Encoding.UTF8, "application/x-www-form-urlencoded");
    }
}
