using BugStore.Domain.Entities;
using BugStore.Domain.Interfaces;
using BugStore.Infra.Data;

namespace BugStore.Infra.Repositories
{
    internal class ProductWriteRepository : IProductWriteRepository
    {
        private readonly BugStoreDbContext _dbContext;

        public ProductWriteRepository(BugStoreDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task AddAsync(Product product)
        {
            await _dbContext.Products.AddAsync(product);
        }

        public async Task UpdateAsync(Product product)
        {
            _dbContext.Products.Update(product);
            await Task.CompletedTask;
        }

        public async Task DeleteAsync(Guid id)
        {
            var product = await _dbContext.Products.FindAsync(id);
            if (product is null)
                return;
            _dbContext.Products.Remove(product);
            await Task.CompletedTask;
        }
    }
}
