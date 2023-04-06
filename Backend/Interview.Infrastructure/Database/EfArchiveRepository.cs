using Interview.Domain.Repository;
using Microsoft.EntityFrameworkCore;

namespace Interview.Infrastructure.Database;

public abstract class EfArchiveRepository<T> : EfRepository<T>, IArchiveRepository<T>
    where T : ArchiveEntity
{
    protected EfArchiveRepository(AppDbContext db)
        : base(db)
    {
    }

    protected override IQueryable<T> ApplyIncludes(DbSet<T> set) => set.Where(it => !it.IsArchived);

    protected override IQueryable<T> DecorateSet(DbSet<T> set) => set.Where(it => !it.IsArchived);
}
