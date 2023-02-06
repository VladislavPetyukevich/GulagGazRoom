using Interview.Domain;
using Microsoft.EntityFrameworkCore;

namespace Interview.Infrastructure.Database;

public class EfRepository<T> : IRepository<T> 
    where T : Entity
{
    private readonly AppDbContext _db;
    private readonly DbSet<T> _set;

    public EfRepository(AppDbContext db)
    {
        _db = db;
        _set = db.Set<T>();
    }

    public Task CreateAsync(T entity, CancellationToken cancellationToken = default)
    {
        _set.Add(entity);
        return _db.SaveChangesAsync(cancellationToken);
    }

    public ValueTask<T?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return _set.FindAsync(id, cancellationToken);
    }

    public Task UpdateAsync(T entity, CancellationToken cancellationToken = default)
    {
        _set.Update(entity);
        return _db.SaveChangesAsync(cancellationToken);
    }

    public Task DeleteAsync(T entity, CancellationToken cancellationToken = default)
    {
        _set.Remove(entity);
        return _db.SaveChangesAsync(cancellationToken);
    }
}