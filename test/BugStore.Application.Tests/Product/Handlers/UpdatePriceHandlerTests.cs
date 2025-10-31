using BugStore.Application.Product.Commands;
using BugStore.Application.Product.Handlers;
using BugStore.Domain.Interfaces;
using BugStore.Exception.ProjectException;
using BugStore.TestUtilities.Builders;
using Moq;

namespace BugStore.Application.Tests.Product.Handlers;

public class UpdatePriceHandlerTests
{
    private readonly Mock<IProductReadOnlyRepository> _readRepositoryMock;
    private readonly Mock<IProductWriteRepository> _writeRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly UpdatePriceHandler _handler;

    public UpdatePriceHandlerTests()
    {
        _readRepositoryMock = new Mock<IProductReadOnlyRepository>();
        _writeRepositoryMock = new Mock<IProductWriteRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _handler = new UpdatePriceHandler(
            _readRepositoryMock.Object,
            _writeRepositoryMock.Object,
            _unitOfWorkMock.Object
        );
    }

    [Fact]
    public async Task Handle_WithValidRequest_ShouldUpdatePriceAndReturnResponse()
    {
        // Arrange
        var productId = Guid.CreateVersion7();
        var newPrice = 99.99m;
        var existingProduct = ProductBuilder.Build();
        var command = new UpdatePriceCommand(productId, newPrice);

        _readRepositoryMock.Setup(x => x.GetByIdAsync(productId)).ReturnsAsync(existingProduct);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(existingProduct.Id, result.Id);
        Assert.Equal(existingProduct.Title, result.Title);
        Assert.Equal(existingProduct.Description, result.Description);
        Assert.Equal(existingProduct.Slug, result.Slug);
        Assert.Equal(newPrice, result.Price);

        _writeRepositoryMock.Verify(x => x.UpdateAsync(existingProduct), Times.Once);
        _unitOfWorkMock.Verify(x => x.CommitAsync(), Times.Once);
    }

    [Fact]
    public async Task Handle_WithNonExistentProduct_ShouldThrowNotFoundException()
    {
        // Arrange
        var productId = Guid.CreateVersion7();
        var newPrice = 99.99m;
        var command = new UpdatePriceCommand(productId, newPrice);

        _readRepositoryMock
            .Setup(x => x.GetByIdAsync(productId))
            .ReturnsAsync((Domain.Entities.Product?)null);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(
            () => _handler.Handle(command, CancellationToken.None).AsTask()
        );

        _writeRepositoryMock.Verify(
            x => x.UpdateAsync(It.IsAny<Domain.Entities.Product>()),
            Times.Never
        );
        _unitOfWorkMock.Verify(x => x.CommitAsync(), Times.Never);
    }

    [Fact]
    public async Task Handle_WithZeroPrice_ShouldUpdatePrice()
    {
        // Arrange
        var productId = Guid.CreateVersion7();
        var newPrice = 0m;
        var existingProduct = ProductBuilder.Build();
        var command = new UpdatePriceCommand(productId, newPrice);

        _readRepositoryMock.Setup(x => x.GetByIdAsync(productId)).ReturnsAsync(existingProduct);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(newPrice, result.Price);

        _writeRepositoryMock.Verify(x => x.UpdateAsync(existingProduct), Times.Once);
        _unitOfWorkMock.Verify(x => x.CommitAsync(), Times.Once);
    }
}
