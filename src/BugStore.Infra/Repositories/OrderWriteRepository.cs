using BugStore.Domain.Entities;
using BugStore.Domain.Interfaces;
using BugStore.Infra.Data;
using Microsoft.EntityFrameworkCore;

namespace BugStore.Infra.Repositories
{
    internal class OrderWriteRepository : IOrderWriteRepository
    {
        private readonly BugStoreDbContext _dbContext;

        public OrderWriteRepository(BugStoreDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task AddAsync(Order order)
        {
            await _dbContext.Orders.AddAsync(order);
        }

        public async Task AddLineAsync(OrderLine orderLine)
        {
            await _dbContext.OrderLines.AddAsync(orderLine);
        }

        public async Task RemoveLineAsync(OrderLine orderLine)
        {
            _dbContext.OrderLines.Remove(orderLine);
            await Task.CompletedTask;
        }

        public async Task DeleteAsync(Guid id)
        {
            var order = await _dbContext
                .Orders.Include(o => o.Lines)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order is null)
                return;

            _dbContext.Orders.Remove(order);
            await Task.CompletedTask;
        }
    }
}
