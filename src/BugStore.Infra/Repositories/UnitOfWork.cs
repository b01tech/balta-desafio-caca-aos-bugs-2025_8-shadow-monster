using BugStore.Domain.Interfaces;
using BugStore.Infra.Data;

namespace BugStore.Infra.Repositories
{
    internal class UnitOfWork : IUnitOfWork
    {
        private readonly BugStoreDbContext _dbContext;

        public UnitOfWork(BugStoreDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task CommitAsync()
        {
            await _dbContext.SaveChangesAsync();
        }
    }
}
