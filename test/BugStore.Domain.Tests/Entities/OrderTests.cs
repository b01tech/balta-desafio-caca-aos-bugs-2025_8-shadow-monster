using BugStore.Domain.Entities;
using BugStore.Exception.ProjectException;
using BugStore.TestUtilities.Builders;

namespace BugStore.Domain.Tests.Entities
{
    public class OrderTests
    {
        [Fact]
        public void GivenValidCustomerId_WhenCreatingOrder_ThenOrderShouldBeCreated()
        {
            // Arrange
            var customerId = Guid.CreateVersion7();

            // Act
            var order = new Order(customerId);

            // Assert
            Assert.NotEqual(Guid.Empty, order.Id);
            Assert.Equal(customerId, order.CustomerId);
            Assert.True(order.CreatedAt <= DateTime.UtcNow);
            Assert.Null(order.UpdatedAt);
            Assert.Empty(order.Lines);
            Assert.Equal(0, order.Total);
        }

        [Fact]
        public void GivenValidData_WhenCreatingOrder_ShouldGenerateUniqueIds()
        {
            // Arrange
            var customerId = Guid.CreateVersion7();

            // Act
            var order1 = new Order(customerId);
            var order2 = OrderBuilder.Build(customerId);

            // Assert
            Assert.NotEqual(Guid.Empty, order1.Id);
            Assert.NotEqual(Guid.Empty, order2.Id);
            Assert.NotEqual(order1.Id, order2.Id);
        }

        [Fact]
        public void GivenValidLine_WhenAddingLine_ThenLineShouldBeAdded()
        {
            // Arrange
            var order = OrderBuilder.Build();
            var productId = Guid.CreateVersion7();
            var quantity = 2;
            var price = 50.00m;

            // Act
            order.AddLine(productId, quantity, price);

            // Assert
            Assert.Single(order.Lines);
            Assert.Equal(productId, order.Lines.First().ProductId);
            Assert.Equal(quantity, order.Lines.First().Quantity);
            Assert.Equal(price * quantity, order.Lines.First().Total);
            Assert.Equal(price * quantity, order.Total);
            Assert.NotNull(order.UpdatedAt);
        }

        [Fact]
        public void GivenMultipleLines_WhenAddingLines_ThenTotalShouldBeCalculatedCorrectly()
        {
            // Arrange
            var order = OrderBuilder.Build();
            var productId1 = Guid.CreateVersion7();
            var productId2 = Guid.CreateVersion7();
            var quantity1 = 2;
            var price1 = 50.00m;
            var quantity2 = 3;
            var price2 = 30.00m;

            // Act
            order.AddLine(productId1, quantity1, price1);
            order.AddLine(productId2, quantity2, price2);

            // Assert
            Assert.Equal(2, order.Lines.Count);
            Assert.Equal((price1 * quantity1) + (price2 * quantity2), order.Total);
        }

        [Fact]
        public void GivenDuplicateProduct_WhenAddingLine_ThenShouldThrowException()
        {
            // Arrange
            var order = OrderBuilder.Build();
            var productId = Guid.CreateVersion7();
            var quantity = 2;
            var price = 50.00m;

            order.AddLine(productId, quantity, price);

            // Act & Assert
            Assert.Throws<OnInvalidOperationException>(
                () => order.AddLine(productId, quantity, price)
            );
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void GivenInvalidQuantity_WhenAddingLine_ThenShouldThrowException(int quantity)
        {
            // Arrange
            var order = OrderBuilder.Build();
            var productId = Guid.CreateVersion7();
            var price = 50.00m;

            // Act & Assert
            Assert.Throws<OnValidationException>(() => order.AddLine(productId, quantity, price));
        }

        [Fact]
        public void GivenExistingLine_WhenRemovingLine_ThenLineShouldBeRemoved()
        {
            // Arrange
            var order = OrderBuilder.BuildWithSpecificLine();
            var lineToRemove = order.Lines.First();
            var originalUpdatedAt = order.UpdatedAt;

            // Act
            order.RemoveLine(lineToRemove);

            // Assert
            Assert.Empty(order.Lines);
            Assert.Equal(0, order.Total);
            Assert.True(order.UpdatedAt > originalUpdatedAt);
        }

        [Fact]
        public void GivenMultipleLines_WhenRemovingOneLine_ThenTotalShouldBeRecalculated()
        {
            // Arrange
            var order = OrderBuilder.Build();
            var productId1 = Guid.CreateVersion7();
            var productId2 = Guid.CreateVersion7();
            var quantity1 = 2;
            var price1 = 50.00m;
            var quantity2 = 3;
            var price2 = 30.00m;

            order.AddLine(productId1, quantity1, price1);
            order.AddLine(productId2, quantity2, price2);

            var lineToRemove = order.Lines.First(l => l.ProductId == productId1);

            // Act
            order.RemoveLine(lineToRemove);

            // Assert
            Assert.Single(order.Lines);
            Assert.Equal(price2 * quantity2, order.Total);
            Assert.Equal(productId2, order.Lines.First().ProductId);
        }
    }
}
