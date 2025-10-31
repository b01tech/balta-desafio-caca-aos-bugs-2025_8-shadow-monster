using BugStore.Application.Product.Handlers;
using BugStore.Application.Product.Queries;
using BugStore.Domain.Interfaces;
using BugStore.Exception.ProjectException;
using BugStore.TestUtilities.Builders;
using Moq;

namespace BugStore.Application.Tests.Product.Handlers;

public class GetProductByIdHandlerTests
{
    private readonly Mock<IProductReadOnlyRepository> _repositoryMock;
    private readonly GetProductByIdHandler _handler;

    public GetProductByIdHandlerTests()
    {
        _repositoryMock = new Mock<IProductReadOnlyRepository>();
        _handler = new GetProductByIdHandler(_repositoryMock.Object);
    }

    [Fact]
    public async Task Handle_WithExistingProduct_ShouldReturnProductData()
    {
        // Arrange
        var productId = Guid.CreateVersion7();
        var existingProduct = ProductBuilder.Build();
        var query = new GetProductByIdQuery(productId);

        _repositoryMock.Setup(x => x.GetByIdAsync(productId)).ReturnsAsync(existingProduct);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(existingProduct.Id, result.Id);
        Assert.Equal(existingProduct.Title, result.Title);
        Assert.Equal(existingProduct.Description, result.Description);
        Assert.Equal(existingProduct.Slug, result.Slug);
        Assert.Equal(existingProduct.Price, result.Price);
    }

    [Fact]
    public async Task Handle_WithNonExistentProduct_ShouldThrowNotFoundException()
    {
        // Arrange
        var productId = Guid.CreateVersion7();
        var query = new GetProductByIdQuery(productId);

        _repositoryMock
            .Setup(x => x.GetByIdAsync(productId))
            .ReturnsAsync((Domain.Entities.Product?)null);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(
            () => _handler.Handle(query, CancellationToken.None).AsTask()
        );
    }
}
