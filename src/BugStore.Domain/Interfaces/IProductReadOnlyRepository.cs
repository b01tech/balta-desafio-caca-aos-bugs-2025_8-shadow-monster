using BugStore.Domain.Entities;

namespace BugStore.Domain.Interfaces
{
    public interface IProductReadOnlyRepository
    {
        Task<Product?> GetByIdAsync(Guid id);
        Task<IEnumerable<Product>> GetAllAsync(int page, int pageSize);
        Task<long> GetTotalItemAsync();
    }
}
