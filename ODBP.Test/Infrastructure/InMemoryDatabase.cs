using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using ODBP.Data;

namespace ODBP.Test.Infrastructure
{
    public static class InMemoryDatabase
    {
        public static OdbpDbContext GetDbContext() => new InMemoryOdbpDbContext(new SqliteConnection("DataSource=:memory:"));

        private sealed class InMemoryOdbpDbContext : OdbpDbContext
        {
            private readonly SqliteConnection _sqliteConnection;

            public InMemoryOdbpDbContext(SqliteConnection sqliteConnection) : base(new DbContextOptionsBuilder<OdbpDbContext>().UseSqlite(sqliteConnection).Options)
            {
                _sqliteConnection = sqliteConnection;
                _sqliteConnection.Open();
                Database.EnsureCreated();
            }

            public override void Dispose()
            {
                _sqliteConnection.Close();
                _sqliteConnection.Dispose();
                base.Dispose();
            }

            public override async ValueTask DisposeAsync()
            {
                await _sqliteConnection.CloseAsync();
                await _sqliteConnection.DisposeAsync();
                await base.DisposeAsync();
            }
        }
    }
}
