using BugStore.Domain.Entities;

namespace BugStore.Domain.Interfaces
{
    public interface ICustomerWriteRepository
    {
        Task AddAsync(Customer customer);
        Task UpdateAsync(Customer customer);
        Task DeleteAsync(Guid id);
    }
}
