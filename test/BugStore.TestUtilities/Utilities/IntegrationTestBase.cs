using BugStore.Infra.Data;
using Microsoft.EntityFrameworkCore;

namespace BugStore.TestUtilities.Utilities
{
    public abstract class IntegrationTestBase : IDisposable
    {
        protected readonly BugStoreDbContext DbContext;
        private bool _disposed = false;

        protected IntegrationTestBase()
        {
            var options = new DbContextOptionsBuilder<BugStoreDbContext>()
                .UseSqlite("Data Source=:memory:")
                .Options;

            DbContext = new BugStoreDbContext(options);
            DbContext.Database.OpenConnection();
            DbContext.Database.EnsureCreated();
        }

        protected async Task ClearDatabaseAsync()
        {
            DbContext.Customers.RemoveRange(DbContext.Customers);
            await DbContext.SaveChangesAsync();
        }

        protected async Task RecreateDatabaseAsync()
        {
            await DbContext.Database.EnsureDeletedAsync();
            await DbContext.Database.EnsureCreatedAsync();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    DbContext.Database.CloseConnection();
                    DbContext.Dispose();
                }
                _disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
