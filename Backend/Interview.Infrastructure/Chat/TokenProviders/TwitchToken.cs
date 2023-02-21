namespace Interview.Infrastructure.Chat.TokenProviders;

public readonly struct TwitchToken : IEquatable<TwitchToken>
{
    public string? AccessToken { get; }

    public DateTime ExpiresIn { get; }

    public bool Expired => DateTime.UtcNow >= ExpiresIn;

    public TwitchToken(string? accessToken, long expiresIn)
        : this(accessToken, DateTime.UtcNow.AddMilliseconds(expiresIn))
    {
    }

    public TwitchToken(string? accessToken, DateTime expiresIn)
    {
        AccessToken = accessToken;
        ExpiresIn = expiresIn;
    }

    public bool Equals(TwitchToken other) => AccessToken == other.AccessToken && ExpiresIn.Equals(other.ExpiresIn);

    public override bool Equals(object? obj) => obj is TwitchToken other && Equals(other);

    public override int GetHashCode() => HashCode.Combine(AccessToken, ExpiresIn);

    public static bool operator ==(TwitchToken left, TwitchToken right) => left.Equals(right);

    public static bool operator !=(TwitchToken left, TwitchToken right) => !(left == right);

    public override string ToString() => $"AccessToken(Token = \"{AccessToken}\", ExpiresIn = \"{ExpiresIn}\", Expired = \"{Expired}\")";
}
