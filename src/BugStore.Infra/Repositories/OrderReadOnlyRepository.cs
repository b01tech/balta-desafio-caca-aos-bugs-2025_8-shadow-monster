using BugStore.Domain.Entities;
using BugStore.Domain.Interfaces;
using BugStore.Infra.Data;
using Microsoft.EntityFrameworkCore;

namespace BugStore.Infra.Repositories
{
    internal class OrderReadOnlyRepository : IOrderReadOnlyRepository
    {
        private readonly BugStoreDbContext _dbContext;

        public OrderReadOnlyRepository(BugStoreDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Order?> GetByIdAsync(Guid id)
        {
            return await _dbContext.Orders
                .Include(o => o.Lines)
                    .ThenInclude(ol => ol.Product)
                .Include(o => o.Customer)
                .FirstOrDefaultAsync(o => o.Id == id);
        }

        public async Task<IEnumerable<Order>> GetAllAsync(int page, int pageSize)
        {
            return await _dbContext.Orders
                .Include(o => o.Lines)
                    .ThenInclude(ol => ol.Product)
                .Include(o => o.Customer)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<IEnumerable<Order>> GetByCustomerIdAsync(Guid customerId, int page, int pageSize)
        {
            return await _dbContext.Orders
                .Include(o => o.Lines)
                    .ThenInclude(ol => ol.Product)
                .Include(o => o.Customer)
                .Where(o => o.CustomerId == customerId)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<long> GetTotalItemAsync()
        {
            return await _dbContext.Orders.CountAsync();
        }
    }
}
