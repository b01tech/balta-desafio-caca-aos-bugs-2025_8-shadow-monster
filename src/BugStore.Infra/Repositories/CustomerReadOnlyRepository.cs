using BugStore.Domain.Entities;
using BugStore.Domain.Interfaces;
using BugStore.Infra.Data;
using Microsoft.EntityFrameworkCore;

namespace BugStore.Infra.Repositories
{
    internal class CustomerReadOnlyRepository : ICustomerReadOnlyRepository
    {
        private readonly BugStoreDbContext _dbContext;

        public CustomerReadOnlyRepository(BugStoreDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<Customer?> GetByIdAsync(Guid id)
        {
            return await _dbContext.Customers.FindAsync(id);
        }
        public async Task<IEnumerable<Customer>> GetAllAsync(int page, int pageSize)
        {
            return await _dbContext.Customers
                .AsNoTracking()
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }
        public async Task<bool> ExistsByEmailAsync(string email)
        {
            return await _dbContext.Customers.AnyAsync(c => c.Email.Equals(email));
        }
        public async Task<long> GetTotalItemAsync()
        {
            return await _dbContext.Customers.CountAsync();
        }
    }
}
