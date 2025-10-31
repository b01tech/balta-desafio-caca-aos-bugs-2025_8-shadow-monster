using BugStore.Domain.Entities;
using BugStore.Infra.Repositories;
using BugStore.TestUtilities.Builders;
using BugStore.TestUtilities.Utilities;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace BugStore.Infra.Tests.Repositories
{
    public class CustomerRepositoryTests : IntegrationTestBase
    {
        private readonly CustomerReadOnlyRepository _readOnlyRepository;
        private readonly CustomerWriteRepository _writeRepository;

        public CustomerRepositoryTests()
        {
            _readOnlyRepository = new CustomerReadOnlyRepository(DbContext);
            _writeRepository = new CustomerWriteRepository(DbContext);
        }

        #region CustomerReadOnlyRepository Tests

        [Fact]
        public async Task GetByIdAsync_WithExistingCustomer_ShouldReturnCustomer()
        {
            // Arrange
            var customer = CustomerBuilder.Build();
            DbContext.Customers.Add(customer);
            await DbContext.SaveChangesAsync();

            // Act
            var result = await _readOnlyRepository.GetByIdAsync(customer.Id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(customer.Id, result.Id);
            Assert.Equal(customer.Name, result.Name);
            Assert.Equal(customer.Email, result.Email);
            Assert.Equal(customer.Phone, result.Phone);
            Assert.Equal(customer.BirthDate, result.BirthDate);
        }

        [Fact]
        public async Task GetByIdAsync_WithNonExistingCustomer_ShouldReturnNull()
        {
            // Arrange
            var nonExistingId = Guid.CreateVersion7();

            // Act
            var result = await _readOnlyRepository.GetByIdAsync(nonExistingId);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetAllAsync_WithNoCustomers_ShouldReturnEmptyList()
        {
            // Arrange
            await ClearDatabaseAsync();

            // Act
            var result = await _readOnlyRepository.GetAllAsync(1, 10);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetAllAsync_WithCustomers_ShouldReturnPagedResults()
        {
            // Arrange
            await ClearDatabaseAsync();
            var customers = new List<Customer>
            {
                CustomerBuilder.Build(name: "Customer 1"),
                CustomerBuilder.Build(name: "Customer 2"),
                CustomerBuilder.Build(name: "Customer 3"),
                CustomerBuilder.Build(name: "Customer 4"),
                CustomerBuilder.Build(name: "Customer 5")
            };

            DbContext.Customers.AddRange(customers);
            await DbContext.SaveChangesAsync();

            // Act
            var firstPage = await _readOnlyRepository.GetAllAsync(1, 2);
            var secondPage = await _readOnlyRepository.GetAllAsync(2, 2);
            var thirdPage = await _readOnlyRepository.GetAllAsync(3, 2);

            // Assert
            Assert.Equal(2, firstPage.Count());
            Assert.Equal(2, secondPage.Count());
            Assert.Single(thirdPage);
        }

        [Fact]
        public async Task GetAllAsync_WithPageSizeLargerThanTotal_ShouldReturnAllCustomers()
        {
            // Arrange
            await ClearDatabaseAsync();
            var customers = new List<Customer>
            {
                CustomerBuilder.Build(),
                CustomerBuilder.Build(),
                CustomerBuilder.Build()
            };

            DbContext.Customers.AddRange(customers);
            await DbContext.SaveChangesAsync();

            // Act
            var result = await _readOnlyRepository.GetAllAsync(1, 10);

            // Assert
            Assert.Equal(3, result.Count());
        }

        [Fact]
        public async Task ExistsByEmailAsync_WithExistingEmail_ShouldReturnTrue()
        {
            // Arrange
            var customer = CustomerBuilder.Build(email: "test@example.com");
            DbContext.Customers.Add(customer);
            await DbContext.SaveChangesAsync();

            // Act
            var result = await _readOnlyRepository.ExistsByEmailAsync("test@example.com");

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task ExistsByEmailAsync_WithNonExistingEmail_ShouldReturnFalse()
        {
            // Arrange
            await ClearDatabaseAsync();

            // Act
            var result = await _readOnlyRepository.ExistsByEmailAsync("nonexisting@example.com");

            // Assert
            Assert.False(result);
        }

        #endregion

        #region CustomerWriteRepository Tests

        [Fact]
        public async Task AddAsync_WithValidCustomer_ShouldAddToDatabase()
        {
            // Arrange
            await ClearDatabaseAsync();
            var customer = CustomerBuilder.Build();

            // Act
            await _writeRepository.AddAsync(customer);
            await DbContext.SaveChangesAsync();

            // Assert
            var savedCustomer = await DbContext.Customers.FindAsync(customer.Id);
            Assert.NotNull(savedCustomer);
            Assert.Equal(customer.Name, savedCustomer.Name);
            Assert.Equal(customer.Email, savedCustomer.Email);
            Assert.Equal(customer.Phone, savedCustomer.Phone);
            Assert.Equal(customer.BirthDate, savedCustomer.BirthDate);
        }

        [Fact]
        public async Task AddAsync_WithMultipleCustomers_ShouldAddAllToDatabase()
        {
            // Arrange
            await ClearDatabaseAsync();
            var customer1 = CustomerBuilder.Build(name: "Customer 1");
            var customer2 = CustomerBuilder.Build(name: "Customer 2");

            // Act
            await _writeRepository.AddAsync(customer1);
            await _writeRepository.AddAsync(customer2);
            await DbContext.SaveChangesAsync();

            // Assert
            var customerCount = await DbContext.Customers.CountAsync();
            Assert.Equal(2, customerCount);

            var savedCustomer1 = await DbContext.Customers.FindAsync(customer1.Id);
            var savedCustomer2 = await DbContext.Customers.FindAsync(customer2.Id);

            Assert.NotNull(savedCustomer1);
            Assert.NotNull(savedCustomer2);
            Assert.Equal("Customer 1", savedCustomer1.Name);
            Assert.Equal("Customer 2", savedCustomer2.Name);
        }

        [Fact]
        public async Task UpdateAsync_WithExistingCustomer_ShouldUpdateInDatabase()
        {
            // Arrange
            var customer = CustomerBuilder.Build(
                name: "Original Name",
                email: "original@example.com"
            );
            DbContext.Customers.Add(customer);
            await DbContext.SaveChangesAsync();

            customer.Update(
                "Updated Name",
                "updated@example.com",
                customer.Phone,
                customer.BirthDate
            );

            // Act
            await _writeRepository.UpdateAsync(customer);
            await DbContext.SaveChangesAsync();

            // Assert
            var updatedCustomer = await DbContext.Customers.FindAsync(customer.Id);
            Assert.NotNull(updatedCustomer);
            Assert.Equal("Updated Name", updatedCustomer.Name);
            Assert.Equal("updated@example.com", updatedCustomer.Email);
        }

        [Fact]
        public async Task DeleteAsync_WithExistingCustomer_ShouldRemoveFromDatabase()
        {
            // Arrange
            var customer = CustomerBuilder.Build();
            DbContext.Customers.Add(customer);
            await DbContext.SaveChangesAsync();

            var existingCustomer = await DbContext.Customers.FindAsync(customer.Id);
            Assert.NotNull(existingCustomer);

            // Act
            await _writeRepository.DeleteAsync(customer.Id);
            await DbContext.SaveChangesAsync();

            // Assert
            var deletedCustomer = await DbContext.Customers.FindAsync(customer.Id);
            Assert.Null(deletedCustomer);
        }

        [Fact]
        public async Task DeleteAsync_WithMultipleCustomers_ShouldDeleteOnlySpecified()
        {
            // Arrange
            await ClearDatabaseAsync();
            var customer1 = CustomerBuilder.Build(name: "Customer 1");
            var customer2 = CustomerBuilder.Build(name: "Customer 2");
            var customer3 = CustomerBuilder.Build(name: "Customer 3");

            DbContext.Customers.AddRange(customer1, customer2, customer3);
            await DbContext.SaveChangesAsync();

            // Act
            await _writeRepository.DeleteAsync(customer2.Id);
            await DbContext.SaveChangesAsync();

            // Assert
            var remainingCustomers = await DbContext.Customers.ToListAsync();
            Assert.Equal(2, remainingCustomers.Count);
            Assert.Contains(remainingCustomers, c => c.Id == customer1.Id);
            Assert.Contains(remainingCustomers, c => c.Id == customer3.Id);
            Assert.DoesNotContain(remainingCustomers, c => c.Id == customer2.Id);
        }

        #endregion

        #region Integration Tests

        [Fact]
        public async Task Integration_AddAndRetrieve_ShouldWorkTogether()
        {
            // Arrange
            await ClearDatabaseAsync();
            var customer = CustomerBuilder.Build();

            // Act - Add
            await _writeRepository.AddAsync(customer);
            await DbContext.SaveChangesAsync();

            // Act - Retrieve
            var retrievedCustomer = await _readOnlyRepository.GetByIdAsync(customer.Id);

            // Assert
            Assert.NotNull(retrievedCustomer);
            Assert.Equal(customer.Id, retrievedCustomer.Id);
            Assert.Equal(customer.Name, retrievedCustomer.Name);
        }

        [Fact]
        public async Task Integration_AddUpdateDelete_ShouldWorkInSequence()
        {
            // Arrange
            await ClearDatabaseAsync();
            var customer = CustomerBuilder.Build(name: "Original Name", email: "original@test.com");

            // Act & Assert - Add
            await _writeRepository.AddAsync(customer);
            await DbContext.SaveChangesAsync();

            var addedCustomer = await _readOnlyRepository.GetByIdAsync(customer.Id);
            Assert.NotNull(addedCustomer);
            Assert.Equal("Original Name", addedCustomer.Name);

            // Act & Assert - Update
            customer.Update("Updated Name", "updated@test.com", customer.Phone, customer.BirthDate);
            await _writeRepository.UpdateAsync(customer);
            await DbContext.SaveChangesAsync();

            var updatedCustomer = await _readOnlyRepository.GetByIdAsync(customer.Id);
            Assert.NotNull(updatedCustomer);
            Assert.Equal("Updated Name", updatedCustomer.Name);
            Assert.Equal("updated@test.com", updatedCustomer.Email);

            // Act & Assert - Delete
            await _writeRepository.DeleteAsync(customer.Id);
            await DbContext.SaveChangesAsync();

            var deletedCustomer = await _readOnlyRepository.GetByIdAsync(customer.Id);
            Assert.Null(deletedCustomer);
        }

        [Fact]
        public async Task Integration_ExistsByEmailAfterAdd_ShouldReturnTrue()
        {
            // Arrange
            await ClearDatabaseAsync();
            var customer = CustomerBuilder.Build(email: "integration@test.com");

            // Act
            await _writeRepository.AddAsync(customer);
            await DbContext.SaveChangesAsync();

            var exists = await _readOnlyRepository.ExistsByEmailAsync("integration@test.com");

            // Assert
            Assert.True(exists);
        }

        [Fact]
        public async Task Integration_GetAllAfterMultipleAdds_ShouldReturnAllCustomers()
        {
            // Arrange
            await ClearDatabaseAsync();
            var customers = new List<Customer>
            {
                CustomerBuilder.Build(name: "Customer A"),
                CustomerBuilder.Build(name: "Customer B"),
                CustomerBuilder.Build(name: "Customer C")
            };

            // Act
            foreach (var customer in customers)
            {
                await _writeRepository.AddAsync(customer);
            }
            await DbContext.SaveChangesAsync();

            var allCustomers = await _readOnlyRepository.GetAllAsync(1, 10);

            // Assert
            Assert.Equal(3, allCustomers.Count());
            Assert.Contains(allCustomers, c => c.Name == "Customer A");
            Assert.Contains(allCustomers, c => c.Name == "Customer B");
            Assert.Contains(allCustomers, c => c.Name == "Customer C");
        }

        #endregion
    }
}
