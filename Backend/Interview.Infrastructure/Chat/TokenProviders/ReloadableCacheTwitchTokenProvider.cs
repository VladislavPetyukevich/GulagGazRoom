using Microsoft.Extensions.Caching.Memory;

namespace Interview.Infrastructure.Chat.TokenProviders;

public sealed class ReloadableCacheTwitchTokenProvider : ITwitchTokenProvider
{
    private const string CacheKey = "ReloadableCacheTwitchTokenProvider";
    
    private readonly ITwitchTokenProvider _original;
    private readonly IMemoryCache _memoryCache;
    private readonly SemaphoreSlim _semaphore;

    public ReloadableCacheTwitchTokenProvider(ITwitchTokenProvider original, IMemoryCache memoryCache)
    {
        _original = original;
        _memoryCache = memoryCache;
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
        if (!_memoryCache.TryGetValue(CacheKey, out TwitchToken twitchToken))
        {
            // Cache miss
            twitchToken = await AddTokenToCacheAsync(cancellationToken);
        }

        if (twitchToken.Expired)
        {
            _memoryCache.Remove(CacheKey);
            twitchToken = await AddTokenToCacheAsync(cancellationToken);
        }

        return twitchToken;
    }

    private async ValueTask<TwitchToken> AddTokenToCacheAsync(CancellationToken cancellationToken)
    {
        var entry = _memoryCache.CreateEntry(CacheKey);
        var token = await _original.GetTokenAsync(cancellationToken);
        entry.Value = token;
        return token;
    }
}