using NSpecifications;

namespace Interview.Domain.Events.Storage;

public sealed class EmptyEventStorage : IEventStorage
{
    public Task AddAsync(IStorageEvent @event, CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    public Task<IReadOnlyCollection<IStorageEvent>> GetBySpecAsync(ISpecification<IStorageEvent> spec, CancellationToken cancellationToken)
    {
        return Task.FromResult<IReadOnlyCollection<IStorageEvent>>(Array.Empty<IStorageEvent>());
    }
}
