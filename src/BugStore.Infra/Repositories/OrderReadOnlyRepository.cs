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
            return await _dbContext
                .Orders.Include(o => o.Lines)
                .ThenInclude(ol => ol.Product)
                .Include(o => o.Customer)
                .FirstOrDefaultAsync(o => o.Id == id);
        }

        public async Task<IEnumerable<Order>> GetAllAsync(int page, int pageSize)
        {
            return await _dbContext
                .Orders.Include(o => o.Lines)
                .ThenInclude(ol => ol.Product)
                .Include(o => o.Customer)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<IEnumerable<Order>> GetByCustomerIdAsync(
            Guid customerId,
            int page,
            int pageSize
        )
        {
            return await _dbContext
                .Orders.Include(o => o.Lines)
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

        public async Task<(long TotalOrders, decimal TotalRevenue)> GetTotalByPeriod(DateTime start, DateTime end)
        {
            var orders = await _dbContext.Orders
                .Where(o => o.CreatedAt >= start && o.CreatedAt <= end)
                .ToListAsync();

            var totalOrders = orders.Count;
            var totalRevenue = orders.Sum(o => o.Total);

            return (totalOrders, totalRevenue);
        }

        public async Task<(long TotalOrders, decimal TotalSpent)> GetTotalByCustomerIdAsync(Guid customerId)
        {
            var orders = await _dbContext.Orders
                .Where(o => o.CustomerId == customerId)
                .ToListAsync();

            var totalOrders = orders.Count;
            var totalSpent = orders.Sum(o => o.Total);

            return (totalOrders, totalSpent);
        }

        public async Task<(Guid CustomerId, long TotalOrders, decimal TotalSpent)> GetBestCustomerBySpentAsync()
        {
            var bestCustomer = await _dbContext.Orders
                .GroupBy(o => o.CustomerId)
                .Select(g => new
                {
                    CustomerId = g.Key,
                    TotalOrders = g.Count(),
                    TotalSpent = g.Sum(o => o.Total)
                })
                .OrderByDescending(x => x.TotalSpent)
                .FirstOrDefaultAsync();

            if (bestCustomer == null)
                return (Guid.Empty, 0, 0);

            return (bestCustomer.CustomerId, bestCustomer.TotalOrders, bestCustomer.TotalSpent);
        }

        public async Task<IEnumerable<(Guid CustomerId, string CustomerName, long TotalOrders, decimal TotalSpent)>> GetBestCustomersAsync(int topCustomers)
        {
            var bestCustomers = await _dbContext.Orders
                .Include(o => o.Customer)
                .GroupBy(o => new { o.CustomerId, o.Customer.Name })
                .Select(g => new
                {
                    CustomerId = g.Key.CustomerId,
                    CustomerName = g.Key.Name,
                    TotalOrders = g.Count(),
                    TotalSpent = g.Sum(o => o.Total)
                })
                .OrderByDescending(x => x.TotalSpent)
                .Take(topCustomers)
                .ToListAsync();

            return bestCustomers.Select(bc => (bc.CustomerId, bc.CustomerName, (long)bc.TotalOrders, bc.TotalSpent));
        }
    }
}
