using System.Collections.Immutable;

namespace Interview.Domain.Connections
{
    public interface IConnectUserSource
    {
        ICollection<Guid> ActiveRooms { get; }

        IReadOnlyDictionary<Guid, (ImmutableHashSet<Guid> Users, string TwitchChanel)> Source { get; }

        void Connect(Guid roomId, Guid userId, string twitchChannel);

        void Disconnect(Guid roomId, Guid userId, string twitchChannel);

        Task WaitAsync(CancellationToken cancellationToken = default);
    }
}
