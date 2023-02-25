using Interview.Infrastructure.Database;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace Interview.Test
{
    public class AppDbContextFixture : IDisposable
    {

        public AppDbContext Context { get; }

        public AppDbContextFixture()
        {
            var sqliteConnection = new SqliteConnection("Data Source=:memory:");
            sqliteConnection.Open();

            var option = new DbContextOptionsBuilder().UseSqlite(
                sqliteConnection
            );

            Context = new AppDbContext(option.Options);

            Context.Database.EnsureCreated();
        }

        public void Dispose()
        {
            Context.Dispose();
        }

    }
}
