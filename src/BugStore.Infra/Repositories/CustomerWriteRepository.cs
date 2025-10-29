using BugStore.Domain.Entities;
using BugStore.Domain.Interfaces;
using BugStore.Infra.Data;

namespace BugStore.Infra.Repositories
{
    internal class CustomerWriteRepository : ICustomerWriteRepository
    {
        private readonly BugStoreDbContext _dbContext;

        public CustomerWriteRepository(BugStoreDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task AddAsync(Customer customer)
        {
            await _dbContext.Customers.AddAsync(customer);
        }
        public async Task UpdateAsync(Customer customer)
        {
            _dbContext.Customers.Update(customer);
            await Task.CompletedTask;
        }
        public async Task DeleteAsync(Guid id)
        {
            var customer = await _dbContext.Customers.FindAsync(id);
            if (customer is null)
                return;
            _dbContext.Customers.Remove(customer);
            await Task.CompletedTask;
        }
    }
}
