using Interview.Infrastructure.Database;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace Interview.Test
{
    public class TestAppDbContextFactory
    {
        public AppDbContext Create()
        {
            var sqliteConnection = new SqliteConnection("Data Source=:memory:");
            sqliteConnection.Open();

            var option = new DbContextOptionsBuilder().UseSqlite(
                sqliteConnection
            );

            var context = new AppDbContext(option.Options);
            context.Database.EnsureCreated();
            return context;
        }
    }
}
