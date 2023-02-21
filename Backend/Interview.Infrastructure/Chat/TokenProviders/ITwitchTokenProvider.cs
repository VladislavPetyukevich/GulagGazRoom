namespace Interview.Infrastructure.Chat.TokenProviders;

public interface ITwitchTokenProvider
{
    ValueTask<TwitchToken> GetTokenAsync(CancellationToken cancellationToken = default);
}
