using BugStore.Domain.Entities;

namespace BugStore.Domain.Interfaces
{
    public interface ICustomerReadOnlyRepository
    {
        Task<Customer?> GetByIdAsync(Guid id);
        Task<IEnumerable<Customer>> GetAllAsync(int page, int pageSize);
        Task<bool> ExistsByEmailAsync(string email);
    }
}
