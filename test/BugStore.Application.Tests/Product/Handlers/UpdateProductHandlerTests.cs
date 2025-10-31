using BugStore.Application.Product.Commands;
using BugStore.Application.Product.Handlers;
using BugStore.Domain.Interfaces;
using BugStore.Exception.ProjectException;
using BugStore.TestUtilities.Builders;
using Moq;

namespace BugStore.Application.Tests.Product.Handlers;

public class UpdateProductHandlerTests
{
    private readonly Mock<IProductReadOnlyRepository> _readRepositoryMock;
    private readonly Mock<IProductWriteRepository> _writeRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly UpdateProductHandler _handler;

    public UpdateProductHandlerTests()
    {
        _readRepositoryMock = new Mock<IProductReadOnlyRepository>();
        _writeRepositoryMock = new Mock<IProductWriteRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _handler = new UpdateProductHandler(
            _readRepositoryMock.Object,
            _writeRepositoryMock.Object,
            _unitOfWorkMock.Object
        );
    }

    [Fact]
    public async Task Handle_WithValidRequest_ShouldUpdateProductAndReturnResponse()
    {
        // Arrange
        var productId = Guid.CreateVersion7();
        var existingProduct = ProductBuilder.Build();
        var request = ProductDTOBuilder.BuildRequest();
        var command = new UpdateProductCommand(productId, request);

        _readRepositoryMock.Setup(x => x.GetByIdAsync(productId)).ReturnsAsync(existingProduct);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(request.Title, result.Title);
        Assert.Equal(request.Description, result.Description);
        Assert.Equal(request.Slug, result.Slug);
        Assert.Equal(request.Price, result.Price);

        _writeRepositoryMock.Verify(x => x.UpdateAsync(existingProduct), Times.Once);
        _unitOfWorkMock.Verify(x => x.CommitAsync(), Times.Once);
    }

    [Fact]
    public async Task Handle_WithNonExistentProduct_ShouldThrowNotFoundException()
    {
        // Arrange
        var productId = Guid.CreateVersion7();
        var request = ProductDTOBuilder.BuildRequest();
        var command = new UpdateProductCommand(productId, request);

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
    public async Task Handle_WithEmptyTitle_ShouldThrowOnValidationException()
    {
        // Arrange
        var productId = Guid.CreateVersion7();
        var request = ProductDTOBuilder.BuildRequest() with { Title = "" };
        var command = new UpdateProductCommand(productId, request);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<OnValidationException>(
            () => _handler.Handle(command, CancellationToken.None).AsTask()
        );

        Assert.NotNull(exception.ErrorMessages);
        Assert.Contains(exception.ErrorMessages, e => e.Contains("Title"));

        _readRepositoryMock.Verify(x => x.GetByIdAsync(It.IsAny<Guid>()), Times.Never);
        _writeRepositoryMock.Verify(
            x => x.UpdateAsync(It.IsAny<Domain.Entities.Product>()),
            Times.Never
        );
        _unitOfWorkMock.Verify(x => x.CommitAsync(), Times.Never);
    }

    [Fact]
    public async Task Handle_WithInvalidPrice_ShouldThrowOnValidationException()
    {
        // Arrange
        var productId = Guid.CreateVersion7();
        var request = ProductDTOBuilder.BuildRequest() with { Price = -5 };
        var command = new UpdateProductCommand(productId, request);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<OnValidationException>(
            () => _handler.Handle(command, CancellationToken.None).AsTask()
        );

        Assert.NotNull(exception.ErrorMessages);
        Assert.Contains(exception.ErrorMessages, e => e.Contains("Price"));

        _readRepositoryMock.Verify(x => x.GetByIdAsync(It.IsAny<Guid>()), Times.Never);
        _writeRepositoryMock.Verify(
            x => x.UpdateAsync(It.IsAny<Domain.Entities.Product>()),
            Times.Never
        );
        _unitOfWorkMock.Verify(x => x.CommitAsync(), Times.Never);
    }
}
