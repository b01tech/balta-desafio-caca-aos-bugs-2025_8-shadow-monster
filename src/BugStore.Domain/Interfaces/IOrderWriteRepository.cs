using BugStore.Domain.Entities;

namespace BugStore.Domain.Interfaces
{
    public interface IOrderWriteRepository
    {
        Task AddAsync(Order order);
        Task UpdateAsync(Order order);
        Task AddLineAsync(OrderLine orderLine);
        Task RemoveLineAsync(OrderLine orderLine);
        Task DeleteAsync(Guid id);
    }
}
