using NSpecifications;

namespace Interview.Domain.Events.Storage;

public interface IEventStorage
{
    Task AddAsync(IStorageEvent @event, CancellationToken cancellationToken);

    IAsyncEnumerable<IReadOnlyCollection<IStorageEvent>> GetBySpecAsync(ISpecification<IStorageEvent> spec, int chunkSize, CancellationToken cancellationToken);
}
