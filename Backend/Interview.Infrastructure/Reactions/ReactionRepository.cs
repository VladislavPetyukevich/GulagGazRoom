using Interview.Domain.Reactions;
using Interview.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace Interview.Infrastructure.Reactions
{
    public class ReactionRepository : EfRepository<Reaction>, IReactionRepository
    {
        public ReactionRepository(AppDbContext db)
            : base(db)
        {
        }

        protected override IQueryable<Reaction> ApplyIncludes(DbSet<Reaction> set) => Set;
    }
}
