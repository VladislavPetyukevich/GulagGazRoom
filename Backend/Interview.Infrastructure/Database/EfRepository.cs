using Interview.Domain;
using Microsoft.EntityFrameworkCore;
using NSpecifications;
using X.PagedList;

namespace Interview.Infrastructure.Database;

public class EfRepository<T> : IRepository<T>
    where T : Entity
{
    protected readonly AppDbContext Db;
    protected readonly DbSet<T> Set;

    public EfRepository(AppDbContext db)
    {
        Db = db;
        Set = db.Set<T>();
    }

    public Task CreateAsync(T entity, CancellationToken cancellationToken = default)
    {
        Set.Add(entity);
        return Db.SaveChangesAsync(cancellationToken);
    }

    public ValueTask<T?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return Set.FindAsync(id, cancellationToken);
    }

    public Task UpdateAsync(T entity, CancellationToken cancellationToken = default)
    {
        Set.Update(entity);
        return Db.SaveChangesAsync(cancellationToken);
    }

    public Task DeleteAsync(T entity, CancellationToken cancellationToken = default)
    {
        Set.Remove(entity);
        return Db.SaveChangesAsync(cancellationToken);
    }

    public Task<IPagedList<T>> GetPage(ISpecification<T> specification, int pageNumber, int pageSize)
    {
        return Set.OrderBy(entity => entity.Id).Where(specification.Expression).ToPagedListAsync(pageNumber, pageSize);
    }

    public Task<IPagedList<T>> GetPage(int pageNumber, int pageSize)
    {
        return Set.OrderBy(entity => entity.Id).ToPagedListAsync(pageNumber, pageSize);
    }
}
