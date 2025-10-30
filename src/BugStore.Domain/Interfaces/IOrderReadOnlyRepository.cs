using BugStore.Domain.Entities;

namespace BugStore.Domain.Interfaces
{
    public interface IOrderReadOnlyRepository
    {
        Task<Order?> GetByIdAsync(Guid id);
        Task<IEnumerable<Order>> GetAllAsync(int page, int pageSize);
        Task<IEnumerable<Order>> GetByCustomerIdAsync(Guid customerId, int page, int pageSize);
        Task<long> GetTotalItemAsync();
    }
}
