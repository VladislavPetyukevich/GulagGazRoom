using Interview.Domain.Users.Roles;
using Interview.Infrastructure.Database;

namespace Interview.Infrastructure.Users;

public class RoleRepository : EfRepository<Role>, IRoleRepository
{
    public RoleRepository(AppDbContext db)
        : base(db)
    {
    }
}
