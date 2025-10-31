using BugStore.Domain.Entities;
using BugStore.Infra.Repositories;
using BugStore.TestUtilities.Builders;
using BugStore.TestUtilities.Utilities;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace BugStore.Infra.Tests.Repositories
{
    public class OrderRepositoryTests : IntegrationTestBase
    {
        private readonly OrderReadOnlyRepository _readOnlyRepository;
        private readonly OrderWriteRepository _writeRepository;

        public OrderRepositoryTests()
        {
            _readOnlyRepository = new OrderReadOnlyRepository(DbContext);
            _writeRepository = new OrderWriteRepository(DbContext);
        }

        #region OrderReadOnlyRepository Tests

        [Fact]
        public async Task GetByIdAsync_WithExistingOrder_ShouldReturnOrderWithIncludes()
        {
            // Arrange
            var customer = CustomerBuilder.Build();
            var product = ProductBuilder.Build();
            var order = OrderBuilder.BuildWithSpecificLine(customer.Id, product.Id);

            DbContext.Customers.Add(customer);
            DbContext.Products.Add(product);
            DbContext.Orders.Add(order);
            await DbContext.SaveChangesAsync();

            // Act
            var result = await _readOnlyRepository.GetByIdAsync(order.Id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(order.Id, result.Id);
            Assert.Equal(order.CustomerId, result.CustomerId);
            Assert.NotNull(result.Customer);
            Assert.Equal(customer.Name, result.Customer.Name);
            Assert.NotEmpty(result.Lines);
            Assert.NotNull(result.Lines.First().Product);
            Assert.Equal(product.Title, result.Lines.First().Product.Title);
        }

        [Fact]
        public async Task GetByIdAsync_WithNonExistingOrder_ShouldReturnNull()
        {
            // Arrange
            var nonExistingId = Guid.CreateVersion7();

            // Act
            var result = await _readOnlyRepository.GetByIdAsync(nonExistingId);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetAllAsync_WithNoOrders_ShouldReturnEmptyList()
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
        public async Task GetAllAsync_WithOrders_ShouldReturnPagedResults()
        {
            // Arrange
            await ClearDatabaseAsync();
            var customer = CustomerBuilder.Build();
            var product = ProductBuilder.Build();
            var orders = new List<Order>
            {
                OrderBuilder.BuildWithSpecificLine(customer.Id, product.Id),
                OrderBuilder.BuildWithSpecificLine(customer.Id, product.Id),
                OrderBuilder.BuildWithSpecificLine(customer.Id, product.Id),
                OrderBuilder.BuildWithSpecificLine(customer.Id, product.Id),
                OrderBuilder.BuildWithSpecificLine(customer.Id, product.Id)
            };

            DbContext.Customers.Add(customer);
            DbContext.Products.Add(product);
            DbContext.Orders.AddRange(orders);
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
        public async Task GetAllAsync_WithPageSizeLargerThanTotal_ShouldReturnAllOrders()
        {
            // Arrange
            await ClearDatabaseAsync();
            var customer = CustomerBuilder.Build();
            var product = ProductBuilder.Build();
            var orders = new List<Order>
            {
                OrderBuilder.BuildWithSpecificLine(customer.Id, product.Id),
                OrderBuilder.BuildWithSpecificLine(customer.Id, product.Id),
                OrderBuilder.BuildWithSpecificLine(customer.Id, product.Id)
            };

            DbContext.Customers.Add(customer);
            DbContext.Products.Add(product);
            DbContext.Orders.AddRange(orders);
            await DbContext.SaveChangesAsync();

            // Act
            var result = await _readOnlyRepository.GetAllAsync(1, 10);

            // Assert
            Assert.Equal(3, result.Count());
        }

        [Fact]
        public async Task GetByCustomerIdAsync_WithExistingCustomer_ShouldReturnCustomerOrders()
        {
            // Arrange
            await ClearDatabaseAsync();
            var customer1 = CustomerBuilder.Build();
            var customer2 = CustomerBuilder.Build();
            var product = ProductBuilder.Build();

            var customer1Orders = new List<Order>
            {
                OrderBuilder.BuildWithSpecificLine(customer1.Id, product.Id),
                OrderBuilder.BuildWithSpecificLine(customer1.Id, product.Id)
            };

            var customer2Order = OrderBuilder.BuildWithSpecificLine(customer2.Id, product.Id);

            DbContext.Customers.AddRange(customer1, customer2);
            DbContext.Products.Add(product);
            DbContext.Orders.AddRange(customer1Orders);
            DbContext.Orders.Add(customer2Order);
            await DbContext.SaveChangesAsync();

            // Act
            var result = await _readOnlyRepository.GetByCustomerIdAsync(customer1.Id, 1, 10);

            // Assert
            Assert.Equal(2, result.Count());
            Assert.All(result, order => Assert.Equal(customer1.Id, order.CustomerId));
        }

        [Fact]
        public async Task GetByCustomerIdAsync_WithNonExistingCustomer_ShouldReturnEmptyList()
        {
            // Arrange
            var nonExistingCustomerId = Guid.CreateVersion7();

            // Act
            var result = await _readOnlyRepository.GetByCustomerIdAsync(
                nonExistingCustomerId,
                1,
                10
            );

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetTotalItemAsync_WithOrders_ShouldReturnCorrectCount()
        {
            // Arrange
            await ClearDatabaseAsync();
            var customer = CustomerBuilder.Build();
            var product = ProductBuilder.Build();
            var orders = new List<Order>
            {
                OrderBuilder.BuildWithSpecificLine(customer.Id, product.Id),
                OrderBuilder.BuildWithSpecificLine(customer.Id, product.Id),
                OrderBuilder.BuildWithSpecificLine(customer.Id, product.Id)
            };

            DbContext.Customers.Add(customer);
            DbContext.Products.Add(product);
            DbContext.Orders.AddRange(orders);
            await DbContext.SaveChangesAsync();

            // Act
            var result = await _readOnlyRepository.GetTotalItemAsync();

            // Assert
            Assert.Equal(3, result);
        }

        [Fact]
        public async Task GetTotalItemAsync_WithNoOrders_ShouldReturnZero()
        {
            // Arrange
            await ClearDatabaseAsync();

            // Act
            var result = await _readOnlyRepository.GetTotalItemAsync();

            // Assert
            Assert.Equal(0, result);
        }

        #endregion

        #region OrderWriteRepository Tests

        [Fact]
        public async Task AddAsync_WithValidOrder_ShouldAddToDatabase()
        {
            // Arrange
            await ClearDatabaseAsync();
            var customer = CustomerBuilder.Build();
            var order = OrderBuilder.Build(customer.Id);

            DbContext.Customers.Add(customer);

            // Act
            await _writeRepository.AddAsync(order);
            await DbContext.SaveChangesAsync();

            // Assert
            var savedOrder = await DbContext.Orders.FindAsync(order.Id);
            Assert.NotNull(savedOrder);
            Assert.Equal(order.CustomerId, savedOrder.CustomerId);
            Assert.Equal(order.CreatedAt, savedOrder.CreatedAt);
        }

        [Fact]
        public async Task AddAsync_WithMultipleOrders_ShouldAddAllToDatabase()
        {
            // Arrange
            await ClearDatabaseAsync();
            var customer = CustomerBuilder.Build();
            var order1 = OrderBuilder.Build(customer.Id);
            var order2 = OrderBuilder.Build(customer.Id);

            DbContext.Customers.Add(customer);

            // Act
            await _writeRepository.AddAsync(order1);
            await _writeRepository.AddAsync(order2);
            await DbContext.SaveChangesAsync();

            // Assert
            var orderCount = await DbContext.Orders.CountAsync();
            Assert.Equal(2, orderCount);

            var savedOrder1 = await DbContext.Orders.FindAsync(order1.Id);
            var savedOrder2 = await DbContext.Orders.FindAsync(order2.Id);

            Assert.NotNull(savedOrder1);
            Assert.NotNull(savedOrder2);
            Assert.Equal(customer.Id, savedOrder1.CustomerId);
            Assert.Equal(customer.Id, savedOrder2.CustomerId);
        }

        [Fact]
        public async Task UpdateAsync_WithExistingOrder_ShouldUpdateInDatabase()
        {
            // Arrange
            var customer = CustomerBuilder.Build();
            var product = ProductBuilder.Build();
            var order = OrderBuilder.Build(customer.Id);

            DbContext.Customers.Add(customer);
            DbContext.Products.Add(product);
            DbContext.Orders.Add(order);
            await DbContext.SaveChangesAsync();

            // Clear change tracker to simulate a fresh context
            DbContext.ChangeTracker.Clear();

            // Reload the order from database to ensure it's tracked
            var trackedOrder = await DbContext
                .Orders.Include(o => o.Lines)
                .FirstAsync(o => o.Id == order.Id);

            // Add a line to trigger update
            trackedOrder.AddLine(product.Id, 2, 100m);

            // Add the line using the repository method
            var orderLine = trackedOrder.Lines.First();
            await _writeRepository.AddLineAsync(orderLine);

            // Act - Just save changes, the entity is already tracked
            await DbContext.SaveChangesAsync();

            // Assert
            var updatedOrder = await DbContext
                .Orders.Include(o => o.Lines)
                .FirstOrDefaultAsync(o => o.Id == order.Id);

            Assert.NotNull(updatedOrder);
            Assert.NotNull(updatedOrder.UpdatedAt);
            Assert.Single(updatedOrder.Lines);
            Assert.Equal(200m, updatedOrder.Total);
        }

        [Fact]
        public async Task DeleteAsync_WithExistingOrder_ShouldRemoveFromDatabase()
        {
            // Arrange
            var customer = CustomerBuilder.Build();
            var order = OrderBuilder.Build(customer.Id);

            DbContext.Customers.Add(customer);
            DbContext.Orders.Add(order);
            await DbContext.SaveChangesAsync();

            var existingOrder = await DbContext.Orders.FindAsync(order.Id);
            Assert.NotNull(existingOrder);

            // Act
            await _writeRepository.DeleteAsync(order.Id);
            await DbContext.SaveChangesAsync();

            // Assert
            var deletedOrder = await DbContext.Orders.FindAsync(order.Id);
            Assert.Null(deletedOrder);
        }

        [Fact]
        public async Task DeleteAsync_WithMultipleOrders_ShouldDeleteOnlySpecified()
        {
            // Arrange
            await ClearDatabaseAsync();
            var customer = CustomerBuilder.Build();
            var order1 = OrderBuilder.Build(customer.Id);
            var order2 = OrderBuilder.Build(customer.Id);
            var order3 = OrderBuilder.Build(customer.Id);

            DbContext.Customers.Add(customer);
            DbContext.Orders.AddRange(order1, order2, order3);
            await DbContext.SaveChangesAsync();

            // Act
            await _writeRepository.DeleteAsync(order2.Id);
            await DbContext.SaveChangesAsync();

            // Assert
            var remainingOrders = await DbContext.Orders.ToListAsync();
            Assert.Equal(2, remainingOrders.Count);
            Assert.Contains(remainingOrders, o => o.Id == order1.Id);
            Assert.Contains(remainingOrders, o => o.Id == order3.Id);
            Assert.DoesNotContain(remainingOrders, o => o.Id == order2.Id);
        }

        [Fact]
        public async Task AddLineAsync_WithValidOrderLine_ShouldAddToDatabase()
        {
            // Arrange
            var customer = CustomerBuilder.Build();
            var product = ProductBuilder.Build();
            var order = OrderBuilder.Build(customer.Id);

            DbContext.Customers.Add(customer);
            DbContext.Products.Add(product);
            DbContext.Orders.Add(order);
            await DbContext.SaveChangesAsync();

            order.AddLine(product.Id, 3, 50m);
            var orderLine = order.Lines.First();

            // Act
            await _writeRepository.AddLineAsync(orderLine);
            await DbContext.SaveChangesAsync();

            // Assert
            var savedOrderLine = await DbContext.OrderLines.FindAsync(orderLine.Id);
            Assert.NotNull(savedOrderLine);
            Assert.Equal(order.Id, savedOrderLine.OrderId);
            Assert.Equal(product.Id, savedOrderLine.ProductId);
            Assert.Equal(3, savedOrderLine.Quantity);
            Assert.Equal(150m, savedOrderLine.Total);
        }

        [Fact]
        public async Task RemoveLineAsync_WithExistingOrderLine_ShouldRemoveFromDatabase()
        {
            // Arrange
            var customer = CustomerBuilder.Build();
            var product = ProductBuilder.Build();
            var order = OrderBuilder.BuildWithSpecificLine(customer.Id, product.Id);

            DbContext.Customers.Add(customer);
            DbContext.Products.Add(product);
            DbContext.Orders.Add(order);
            await DbContext.SaveChangesAsync();

            var orderLine = order.Lines.First();

            // Act
            await _writeRepository.RemoveLineAsync(orderLine);
            await DbContext.SaveChangesAsync();

            // Assert
            var deletedOrderLine = await DbContext.OrderLines.FindAsync(orderLine.Id);
            Assert.Null(deletedOrderLine);
        }

        #endregion

        #region Integration Tests

        [Fact]
        public async Task Integration_AddAndRetrieve_ShouldWorkTogether()
        {
            // Arrange
            await ClearDatabaseAsync();
            var customer = CustomerBuilder.Build();
            var product = ProductBuilder.Build();
            var order = OrderBuilder.BuildWithSpecificLine(customer.Id, product.Id);

            DbContext.Customers.Add(customer);
            DbContext.Products.Add(product);

            // Act - Add
            await _writeRepository.AddAsync(order);
            await DbContext.SaveChangesAsync();

            // Act - Retrieve
            var retrievedOrder = await _readOnlyRepository.GetByIdAsync(order.Id);

            // Assert
            Assert.NotNull(retrievedOrder);
            Assert.Equal(order.Id, retrievedOrder.Id);
            Assert.Equal(order.CustomerId, retrievedOrder.CustomerId);
            Assert.NotNull(retrievedOrder.Customer);
            Assert.NotEmpty(retrievedOrder.Lines);
        }

        [Fact]
        public async Task Integration_AddUpdateDelete_ShouldWorkInSequence()
        {
            // Arrange
            await ClearDatabaseAsync();
            var customer = CustomerBuilder.Build();
            var product = ProductBuilder.Build();
            var order = OrderBuilder.Build(customer.Id);

            DbContext.Customers.Add(customer);
            DbContext.Products.Add(product);

            // Act & Assert - Add
            await _writeRepository.AddAsync(order);
            await DbContext.SaveChangesAsync();

            var addedOrder = await _readOnlyRepository.GetByIdAsync(order.Id);
            Assert.NotNull(addedOrder);
            Assert.Equal(0, addedOrder.Total);

            // Act & Assert - Update (add line)
            // Clear change tracker to simulate a fresh context
            DbContext.ChangeTracker.Clear();

            // Reload the order to ensure proper tracking
            var trackedOrder = await DbContext
                .Orders.Include(o => o.Lines)
                .FirstAsync(o => o.Id == order.Id);

            trackedOrder.AddLine(product.Id, 2, 75m);

            // Add the line using the repository method
            var orderLine = trackedOrder.Lines.First();
            await _writeRepository.AddLineAsync(orderLine);

            await DbContext.SaveChangesAsync();

            var updatedOrder = await _readOnlyRepository.GetByIdAsync(order.Id);
            Assert.NotNull(updatedOrder);
            Assert.Equal(150m, updatedOrder.Total);
            Assert.Single(updatedOrder.Lines);

            // Act & Assert - Delete
            await _writeRepository.DeleteAsync(order.Id);
            await DbContext.SaveChangesAsync();

            var deletedOrder = await _readOnlyRepository.GetByIdAsync(order.Id);
            Assert.Null(deletedOrder);
        }

        [Fact]
        public async Task Integration_GetByCustomerAfterAdd_ShouldReturnCustomerOrders()
        {
            // Arrange
            await ClearDatabaseAsync();
            var customer = CustomerBuilder.Build();
            var product = ProductBuilder.Build();
            var order = OrderBuilder.BuildWithSpecificLine(customer.Id, product.Id);

            DbContext.Customers.Add(customer);
            DbContext.Products.Add(product);

            // Act
            await _writeRepository.AddAsync(order);
            await DbContext.SaveChangesAsync();

            var customerOrders = await _readOnlyRepository.GetByCustomerIdAsync(customer.Id, 1, 10);

            // Assert
            Assert.Single(customerOrders);
            Assert.Equal(order.Id, customerOrders.First().Id);
        }

        [Fact]
        public async Task Integration_GetTotalAfterMultipleAdds_ShouldReturnCorrectCount()
        {
            // Arrange
            await ClearDatabaseAsync();
            var customer = CustomerBuilder.Build();
            var product = ProductBuilder.Build();
            var orders = new List<Order>
            {
                OrderBuilder.BuildWithSpecificLine(customer.Id, product.Id),
                OrderBuilder.BuildWithSpecificLine(customer.Id, product.Id),
                OrderBuilder.BuildWithSpecificLine(customer.Id, product.Id)
            };

            DbContext.Customers.Add(customer);
            DbContext.Products.Add(product);

            // Act
            foreach (var order in orders)
            {
                await _writeRepository.AddAsync(order);
            }
            await DbContext.SaveChangesAsync();

            var totalCount = await _readOnlyRepository.GetTotalItemAsync();

            // Assert
            Assert.Equal(3, totalCount);
        }

        [Fact]
        public async Task Integration_AddAndRemoveOrderLine_ShouldWorkTogether()
        {
            // Arrange
            await ClearDatabaseAsync();
            var customer = CustomerBuilder.Build();
            var product = ProductBuilder.Build();
            var order = OrderBuilder.Build(customer.Id);

            DbContext.Customers.Add(customer);
            DbContext.Products.Add(product);
            DbContext.Orders.Add(order);
            await DbContext.SaveChangesAsync();

            // Act - Add line
            order.AddLine(product.Id, 2, 100m);
            var orderLine = order.Lines.First();
            await _writeRepository.AddLineAsync(orderLine);
            await DbContext.SaveChangesAsync();

            var orderWithLine = await _readOnlyRepository.GetByIdAsync(order.Id);
            Assert.NotNull(orderWithLine);
            Assert.Single(orderWithLine.Lines);

            // Act - Remove line
            await _writeRepository.RemoveLineAsync(orderLine);
            await DbContext.SaveChangesAsync();

            var orderWithoutLine = await _readOnlyRepository.GetByIdAsync(order.Id);
            Assert.NotNull(orderWithoutLine);
            Assert.Empty(orderWithoutLine.Lines);
        }

        #endregion
    }
}
