using BugStore.Domain.Entities;
using BugStore.Domain.Interfaces;
using BugStore.Infra.Data;
using Microsoft.EntityFrameworkCore;

namespace BugStore.Infra.Repositories
{
    internal class ProductReadOnlyRepository : IProductReadOnlyRepository
    {
        private readonly BugStoreDbContext _dbContext;

        public ProductReadOnlyRepository(BugStoreDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Product?> GetByIdAsync(Guid id)
        {
            return await _dbContext.Products.FindAsync(id);
        }

        public async Task<IEnumerable<Product>> GetAllAsync(int page, int pageSize)
        {
            return await _dbContext.Products
                .AsNoTracking()
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<long> GetTotalItemAsync()
        {
            return await _dbContext.Products.CountAsync();
        }
    }
}
