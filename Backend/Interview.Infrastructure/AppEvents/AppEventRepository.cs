using Interview.Domain.Events;
using Interview.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace Interview.Infrastructure.AppEvents
{
    public class AppEventRepository : EfRepository<AppEvent>, IAppEventRepository
    {
        public AppEventRepository(AppDbContext db)
            : base(db)
        {
        }

        protected override IQueryable<AppEvent> ApplyDetailed(DbSet<AppEvent> set)
            => set.Include(e => e.Roles);
    }
}
