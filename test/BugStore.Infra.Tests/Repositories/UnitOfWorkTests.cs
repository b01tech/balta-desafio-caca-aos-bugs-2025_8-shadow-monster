using BugStore.Infra.Repositories;
using Microsoft.EntityFrameworkCore;
using BugStore.TestUtilities.Utilities;
using BugStore.TestUtilities.Builders;

namespace BugStore.Infra.Tests.Repositories
{
    public class UnitOfWorkTests : IntegrationTestBase
    {
        private readonly UnitOfWork _unitOfWork;

        public UnitOfWorkTests()
        {
            _unitOfWork = new UnitOfWork(DbContext);
        }

        [Fact]
        public void UnitOfWork_ShouldBeCreatedSuccessfully()
        {
            // Arrange & Act

            // Assert
            Assert.NotNull(_unitOfWork);
        }

        [Fact]
        public async Task CommitAsync_WithNoChanges_ShouldCompleteSuccessfully()
        {
            // Arrange
            // No changes

            // Act
            await _unitOfWork.CommitAsync();

            // Assert
            Assert.True(true);
        }

        [Fact]
        public async Task CommitAsync_WithSingleCustomer_ShouldSaveToDatabase()
        {
            // Arrange
            var customer = CustomerBuilder.Build();
            DbContext.Customers.Add(customer);

            // Act
            await _unitOfWork.CommitAsync();

            // Assert
            var savedCustomer = await DbContext.Customers.FirstOrDefaultAsync(c => c.Id == customer.Id);
            Assert.NotNull(savedCustomer);
            Assert.Equal(customer.Name, savedCustomer.Name);
            Assert.Equal(customer.Email, savedCustomer.Email);
            Assert.Equal(customer.BirthDate, savedCustomer.BirthDate);
        }

        [Fact]
        public async Task CommitAsync_WithMultipleCustomers_ShouldSaveAllToDatabase()
        {
            // Arrange
            var customer1 = CustomerBuilder.Build();
            var customer2 = CustomerBuilder.Build();

            DbContext.Customers.AddRange(customer1, customer2);

            // Act
            await _unitOfWork.CommitAsync();

            // Assert
            var savedCustomers = await DbContext.Customers.ToListAsync();
            Assert.Equal(2, savedCustomers.Count);
            Assert.Contains(savedCustomers, c => c.Name == customer1.Name);
            Assert.Contains(savedCustomers, c => c.Name == customer2.Name);
        }

        [Fact]
        public async Task CommitAsync_WithUpdatedCustomer_ShouldSaveChangesToDatabase()
        {
            // Arrange
            var customer = CustomerBuilder.Build();
            DbContext.Customers.Add(customer);
            await _unitOfWork.CommitAsync();

            customer.Update("Updated Name", "updated@example.com", "12345678900", DateTime.Now);

            // Act
            await _unitOfWork.CommitAsync();

            // Assert
            var updatedCustomer = await DbContext.Customers.FirstOrDefaultAsync(c => c.Id == customer.Id);
            Assert.NotNull(updatedCustomer);
            Assert.Equal("Updated Name", updatedCustomer.Name);
            Assert.Equal("updated@example.com", updatedCustomer.Email);
            Assert.Equal("12345678900", updatedCustomer.Phone);
        }

        [Fact]
        public async Task CommitAsync_WithDeletedCustomer_ShouldRemoveFromDatabase()
        {
            // Arrange
            var customer = CustomerBuilder.Build();
            DbContext.Customers.Add(customer);
            await _unitOfWork.CommitAsync();

            // Remover o customer
            DbContext.Customers.Remove(customer);

            // Act
            await _unitOfWork.CommitAsync();

            // Assert
            var deletedCustomer = await DbContext.Customers.FirstOrDefaultAsync(c => c.Id == customer.Id);
            Assert.Null(deletedCustomer);
        }
    }
}
