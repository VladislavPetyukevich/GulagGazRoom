using Interview.Domain.Events.ChangeEntityProcessors;
using Interview.Domain.Users;
using Interview.Infrastructure.Database;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Internal;

namespace Interview.Test;

public class TestAppDbContextFactory
{
    public AppDbContext Create(ISystemClock clock)
    {
        var sqliteConnection = new SqliteConnection("Data Source=:memory:");
        sqliteConnection.Open();

        var option = new DbContextOptionsBuilder().UseSqlite(
            sqliteConnection
        );

        var context = new AppDbContext(option.Options, Array.Empty<IChangeEntityProcessor>())
        {
            SystemClock = clock,
            LazyCurrentUserAccessor = () => null
        };
        context.Database.EnsureCreated();
        return context;
    }
}
