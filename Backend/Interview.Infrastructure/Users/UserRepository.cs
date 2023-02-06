using Interview.Domain.Users;
using Interview.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace Interview.Infrastructure.Users;

public class UserRepository: EfRepository<User>, IUserRepository
{
    public UserRepository(AppDbContext db) : base(db)
    {
    }

    public Task<User?> FindByNicknameAsync(string nickname, CancellationToken cancellationToken = default)
    {
        return _set.FirstOrDefaultAsync(user => user.Nickname == nickname, cancellationToken);
    }
    
}