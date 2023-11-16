using NSpecifications;

namespace Interview.Domain.Events.Storage;

public sealed class EmptyEventStorage : IEventStorage
{
    public Task AddAsync(IStorageEvent @event, CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    public IAsyncEnumerable<IReadOnlyCollection<IStorageEvent>> GetBySpecAsync(ISpecification<IStorageEvent> spec, int chunkSize, CancellationToken cancellationToken)
    {
        return AsyncEnumerable.Empty<IReadOnlyCollection<IStorageEvent>>();
    }
}
