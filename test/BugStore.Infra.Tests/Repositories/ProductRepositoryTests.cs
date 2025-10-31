using BugStore.Domain.Entities;
using BugStore.Infra.Repositories;
using BugStore.TestUtilities.Builders;
using BugStore.TestUtilities.Utilities;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace BugStore.Infra.Tests.Repositories
{
    public class ProductRepositoryTests : IntegrationTestBase
    {
        private readonly ProductReadOnlyRepository _readOnlyRepository;
        private readonly ProductWriteRepository _writeRepository;

        public ProductRepositoryTests()
        {
            _readOnlyRepository = new ProductReadOnlyRepository(DbContext);
            _writeRepository = new ProductWriteRepository(DbContext);
        }

        #region ProductReadOnlyRepository Tests

        [Fact]
        public async Task GetByIdAsync_WithExistingProduct_ShouldReturnProduct()
        {
            // Arrange
            var product = ProductBuilder.Build();
            DbContext.Products.Add(product);
            await DbContext.SaveChangesAsync();

            // Act
            var result = await _readOnlyRepository.GetByIdAsync(product.Id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(product.Id, result.Id);
            Assert.Equal(product.Title, result.Title);
            Assert.Equal(product.Description, result.Description);
            Assert.Equal(product.Slug, result.Slug);
            Assert.Equal(product.Price, result.Price);
        }

        [Fact]
        public async Task GetByIdAsync_WithNonExistingProduct_ShouldReturnNull()
        {
            // Arrange
            var nonExistingId = Guid.CreateVersion7();

            // Act
            var result = await _readOnlyRepository.GetByIdAsync(nonExistingId);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetAllAsync_WithNoProducts_ShouldReturnEmptyList()
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
        public async Task GetAllAsync_WithProducts_ShouldReturnPagedResults()
        {
            // Arrange
            await ClearDatabaseAsync();
            var products = new List<Product>
            {
                ProductBuilder.Build(title: "Product 1"),
                ProductBuilder.Build(title: "Product 2"),
                ProductBuilder.Build(title: "Product 3"),
                ProductBuilder.Build(title: "Product 4"),
                ProductBuilder.Build(title: "Product 5")
            };

            DbContext.Products.AddRange(products);
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
        public async Task GetAllAsync_WithPageSizeLargerThanTotal_ShouldReturnAllProducts()
        {
            // Arrange
            await ClearDatabaseAsync();
            var products = new List<Product>
            {
                ProductBuilder.Build(),
                ProductBuilder.Build(),
                ProductBuilder.Build()
            };

            DbContext.Products.AddRange(products);
            await DbContext.SaveChangesAsync();

            // Act
            var result = await _readOnlyRepository.GetAllAsync(1, 10);

            // Assert
            Assert.Equal(3, result.Count());
        }

        [Fact]
        public async Task GetTotalItemAsync_WithProducts_ShouldReturnCorrectCount()
        {
            // Arrange
            await ClearDatabaseAsync();
            var products = new List<Product>
            {
                ProductBuilder.Build(),
                ProductBuilder.Build(),
                ProductBuilder.Build()
            };

            DbContext.Products.AddRange(products);
            await DbContext.SaveChangesAsync();

            // Act
            var result = await _readOnlyRepository.GetTotalItemAsync();

            // Assert
            Assert.Equal(3, result);
        }

        [Fact]
        public async Task GetTotalItemAsync_WithNoProducts_ShouldReturnZero()
        {
            // Arrange
            await ClearDatabaseAsync();

            // Act
            var result = await _readOnlyRepository.GetTotalItemAsync();

            // Assert
            Assert.Equal(0, result);
        }

        #endregion

        #region ProductWriteRepository Tests

        [Fact]
        public async Task AddAsync_WithValidProduct_ShouldAddToDatabase()
        {
            // Arrange
            await ClearDatabaseAsync();
            var product = ProductBuilder.Build();

            // Act
            await _writeRepository.AddAsync(product);
            await DbContext.SaveChangesAsync();

            // Assert
            var savedProduct = await DbContext.Products.FindAsync(product.Id);
            Assert.NotNull(savedProduct);
            Assert.Equal(product.Title, savedProduct.Title);
            Assert.Equal(product.Description, savedProduct.Description);
            Assert.Equal(product.Slug, savedProduct.Slug);
            Assert.Equal(product.Price, savedProduct.Price);
        }

        [Fact]
        public async Task AddAsync_WithMultipleProducts_ShouldAddAllToDatabase()
        {
            // Arrange
            await ClearDatabaseAsync();
            var product1 = ProductBuilder.Build(title: "Product 1");
            var product2 = ProductBuilder.Build(title: "Product 2");

            // Act
            await _writeRepository.AddAsync(product1);
            await _writeRepository.AddAsync(product2);
            await DbContext.SaveChangesAsync();

            // Assert
            var productCount = await DbContext.Products.CountAsync();
            Assert.Equal(2, productCount);

            var savedProduct1 = await DbContext.Products.FindAsync(product1.Id);
            var savedProduct2 = await DbContext.Products.FindAsync(product2.Id);

            Assert.NotNull(savedProduct1);
            Assert.NotNull(savedProduct2);
            Assert.Equal("Product 1", savedProduct1.Title);
            Assert.Equal("Product 2", savedProduct2.Title);
        }

        [Fact]
        public async Task UpdateAsync_WithExistingProduct_ShouldUpdateInDatabase()
        {
            // Arrange
            var product = ProductBuilder.Build(
                title: "Original Title",
                description: "Original Description",
                slug: "original-slug",
                price: 100m
            );

            DbContext.Products.Add(product);
            await DbContext.SaveChangesAsync();

            product.Update("Updated Title", "Updated Description", "updated-slug", 200m);

            // Act
            await _writeRepository.UpdateAsync(product);
            await DbContext.SaveChangesAsync();

            // Assert
            var updatedProduct = await DbContext.Products.FindAsync(product.Id);
            Assert.NotNull(updatedProduct);
            Assert.Equal("Updated Title", updatedProduct.Title);
            Assert.Equal("Updated Description", updatedProduct.Description);
            Assert.Equal("updated-slug", updatedProduct.Slug);
            Assert.Equal(200m, updatedProduct.Price);
        }

        [Fact]
        public async Task DeleteAsync_WithExistingProduct_ShouldRemoveFromDatabase()
        {
            // Arrange
            var product = ProductBuilder.Build();
            DbContext.Products.Add(product);
            await DbContext.SaveChangesAsync();

            var existingProduct = await DbContext.Products.FindAsync(product.Id);
            Assert.NotNull(existingProduct);

            // Act
            await _writeRepository.DeleteAsync(product.Id);
            await DbContext.SaveChangesAsync();

            // Assert
            var deletedProduct = await DbContext.Products.FindAsync(product.Id);
            Assert.Null(deletedProduct);
        }

        [Fact]
        public async Task DeleteAsync_WithMultipleProducts_ShouldDeleteOnlySpecified()
        {
            // Arrange
            await ClearDatabaseAsync();
            var product1 = ProductBuilder.Build(title: "Product 1");
            var product2 = ProductBuilder.Build(title: "Product 2");
            var product3 = ProductBuilder.Build(title: "Product 3");

            DbContext.Products.AddRange(product1, product2, product3);
            await DbContext.SaveChangesAsync();

            // Act
            await _writeRepository.DeleteAsync(product2.Id);
            await DbContext.SaveChangesAsync();

            // Assert
            var remainingProducts = await DbContext.Products.ToListAsync();
            Assert.Equal(2, remainingProducts.Count);
            Assert.Contains(remainingProducts, p => p.Id == product1.Id);
            Assert.Contains(remainingProducts, p => p.Id == product3.Id);
            Assert.DoesNotContain(remainingProducts, p => p.Id == product2.Id);
        }

        #endregion

        #region Integration Tests

        [Fact]
        public async Task Integration_AddAndRetrieve_ShouldWorkTogether()
        {
            // Arrange
            await ClearDatabaseAsync();
            var product = ProductBuilder.Build();

            // Act - Add
            await _writeRepository.AddAsync(product);
            await DbContext.SaveChangesAsync();

            // Act - Retrieve
            var retrievedProduct = await _readOnlyRepository.GetByIdAsync(product.Id);

            // Assert
            Assert.NotNull(retrievedProduct);
            Assert.Equal(product.Id, retrievedProduct.Id);
            Assert.Equal(product.Title, retrievedProduct.Title);
        }

        [Fact]
        public async Task Integration_AddUpdateDelete_ShouldWorkInSequence()
        {
            // Arrange
            await ClearDatabaseAsync();
            var product = ProductBuilder.Build(
                title: "Original Title",
                description: "Original Description",
                slug: "original-slug",
                price: 100m
            );

            // Act & Assert - Add
            await _writeRepository.AddAsync(product);
            await DbContext.SaveChangesAsync();

            var addedProduct = await _readOnlyRepository.GetByIdAsync(product.Id);
            Assert.NotNull(addedProduct);
            Assert.Equal("Original Title", addedProduct.Title);
            Assert.Equal(100m, addedProduct.Price);

            // Act & Assert - Update
            product.Update("Updated Title", "Updated Description", "updated-slug", 200m);
            await _writeRepository.UpdateAsync(product);
            await DbContext.SaveChangesAsync();

            var updatedProduct = await _readOnlyRepository.GetByIdAsync(product.Id);
            Assert.NotNull(updatedProduct);
            Assert.Equal("Updated Title", updatedProduct.Title);
            Assert.Equal("Updated Description", updatedProduct.Description);
            Assert.Equal("updated-slug", updatedProduct.Slug);
            Assert.Equal(200m, updatedProduct.Price);

            // Act & Assert - Delete
            await _writeRepository.DeleteAsync(product.Id);
            await DbContext.SaveChangesAsync();

            var deletedProduct = await _readOnlyRepository.GetByIdAsync(product.Id);
            Assert.Null(deletedProduct);
        }

        [Fact]
        public async Task Integration_GetAllAfterMultipleAdds_ShouldReturnAllProducts()
        {
            // Arrange
            await ClearDatabaseAsync();
            var products = new List<Product>
            {
                ProductBuilder.Build(title: "Product A"),
                ProductBuilder.Build(title: "Product B"),
                ProductBuilder.Build(title: "Product C")
            };

            // Act
            foreach (var product in products)
            {
                await _writeRepository.AddAsync(product);
            }
            await DbContext.SaveChangesAsync();

            var allProducts = await _readOnlyRepository.GetAllAsync(1, 10);

            // Assert
            Assert.Equal(3, allProducts.Count());
            Assert.Contains(allProducts, p => p.Title == "Product A");
            Assert.Contains(allProducts, p => p.Title == "Product B");
            Assert.Contains(allProducts, p => p.Title == "Product C");
        }

        [Fact]
        public async Task Integration_GetTotalAfterMultipleAdds_ShouldReturnCorrectCount()
        {
            // Arrange
            await ClearDatabaseAsync();
            var products = new List<Product>
            {
                ProductBuilder.Build(),
                ProductBuilder.Build(),
                ProductBuilder.Build(),
                ProductBuilder.Build()
            };

            // Act
            foreach (var product in products)
            {
                await _writeRepository.AddAsync(product);
            }
            await DbContext.SaveChangesAsync();

            var totalCount = await _readOnlyRepository.GetTotalItemAsync();

            // Assert
            Assert.Equal(4, totalCount);
        }

        [Fact]
        public async Task Integration_PaginationConsistency_ShouldWorkCorrectly()
        {
            // Arrange
            await ClearDatabaseAsync();
            var products = new List<Product>();
            for (int i = 1; i <= 10; i++)
            {
                products.Add(ProductBuilder.Build(title: $"Product {i:D2}"));
            }

            foreach (var product in products)
            {
                await _writeRepository.AddAsync(product);
            }
            await DbContext.SaveChangesAsync();

            // Act
            var page1 = await _readOnlyRepository.GetAllAsync(1, 3);
            var page2 = await _readOnlyRepository.GetAllAsync(2, 3);
            var page3 = await _readOnlyRepository.GetAllAsync(3, 3);
            var page4 = await _readOnlyRepository.GetAllAsync(4, 3);
            var totalCount = await _readOnlyRepository.GetTotalItemAsync();

            // Assert
            Assert.Equal(3, page1.Count());
            Assert.Equal(3, page2.Count());
            Assert.Equal(3, page3.Count());
            Assert.Single(page4);
            Assert.Equal(10, totalCount);

            // Verify no duplicates across pages
            var allPagedProducts = page1.Concat(page2).Concat(page3).Concat(page4).ToList();
            var uniqueIds = allPagedProducts.Select(p => p.Id).Distinct().Count();
            Assert.Equal(10, uniqueIds);
        }

        #endregion
    }
}
