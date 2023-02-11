namespace Interview.Infrastructure.Chat.TokenProviders;

public sealed class ReloadableCacheTwitchTokenProvider : ITwitchTokenProvider
{
    private TwitchToken? _cacheToken;
    private readonly ITwitchTokenProvider _original;
    private readonly SemaphoreSlim _semaphore;

    public ReloadableCacheTwitchTokenProvider(ITwitchTokenProvider original)
    {
        _original = original;
        _semaphore = new SemaphoreSlim(1, 1);
    }

    public async ValueTask<TwitchToken> GetTokenAsync(CancellationToken cancellationToken = default)
    {
        await _semaphore.WaitAsync(cancellationToken);

        try
        {
            // Non thread safe
            return await GetTokenCoreAsync(cancellationToken);
        }
        finally
        {
            _semaphore.Release();
        }
    }

    private async ValueTask<TwitchToken> GetTokenCoreAsync(CancellationToken cancellationToken)
    {
        _cacheToken ??= await _original.GetTokenAsync(cancellationToken);
        var twitchToken = _cacheToken.Value;
        if (!twitchToken.Expired) 
            return twitchToken;
        // Refresh
        _cacheToken = twitchToken = await _original.GetTokenAsync(cancellationToken);
        return twitchToken;
    }
}