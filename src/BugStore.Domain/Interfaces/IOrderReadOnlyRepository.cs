using BugStore.Domain.Entities;

namespace BugStore.Domain.Interfaces
{
    public interface IOrderReadOnlyRepository
    {
        Task<Order?> GetByIdAsync(Guid id);
        Task<IEnumerable<Order>> GetAllAsync(int page, int pageSize);
        Task<IEnumerable<Order>> GetByCustomerIdAsync(Guid customerId, int page, int pageSize);
        Task<long> GetTotalItemAsync();
        Task<(long TotalOrders, decimal TotalRevenue)> GetTotalByPeriod(DateTime start, DateTime end);
        Task<(long TotalOrders, decimal TotalSpent)> GetTotalByCustomerIdAsync(Guid customerId);
        Task<(Guid CustomerId, long TotalOrders, decimal TotalSpent)> GetBestCustomerBySpentAsync();
        Task<IEnumerable<(Guid CustomerId, string CustomerName, long TotalOrders, decimal TotalSpent)>> GetBestCustomersAsync(int topCustomers);
    }
}
