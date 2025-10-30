using BugStore.Domain.Entities;

namespace BugStore.Domain.Interfaces
{
    public interface IProductWriteRepository
    {
        Task AddAsync(Product product);
        Task UpdateAsync(Product product);
        Task DeleteAsync(Guid id);
    }
}
