using BugStore.Domain.Entities;
using BugStore.Exception.ProjectException;
using BugStore.TestUtilities.Builders;

namespace BugStore.Domain.Tests.Entities
{
    public class ProductTests
    {
        [Fact]
        public void GivenValidData_WhenCreatingProduct_ThenProductShouldBeCreated()
        {
            // Arrange
            var title = "ValidTitle";
            var description = "ValidDescription";
            var slug = "valid-title";
            var price = 99.99m;

            // Act
            var product = new Product(title, description, slug, price);

            // Assert
            Assert.NotEqual(Guid.Empty, product.Id);
            Assert.Equal(title, product.Title);
            Assert.Equal(description, product.Description);
            Assert.Equal(slug, product.Slug);
            Assert.Equal(price, product.Price);
        }

        [Fact]
        public void GivenValidData_WhenCreatingProduct_ShouldGenerateUniqueIds()
        {
            // Arrange
            var product = ProductBuilder.Build();
            var anotherProduct = new Product(product.Title, product.Description, product.Slug, product.Price);

            // Act & Assert
            Assert.NotEqual(Guid.Empty, product.Id);
            Assert.NotEqual(Guid.Empty, anotherProduct.Id);
            Assert.NotEqual(product.Id, anotherProduct.Id);
        }

        [Fact]
        public void GivenNegativePrice_WhenCreatingProduct_ThenThrow()
        {
            // Arrange
            var price = -1m;

            // Act & Assert
            Assert.Throws<OnValidationException>(() => ProductBuilder.Build(price: price));
        }


    }
}
