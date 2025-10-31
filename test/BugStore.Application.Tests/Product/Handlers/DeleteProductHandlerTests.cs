using BugStore.Application.Product.Commands;
using BugStore.Application.Product.Handlers;
using BugStore.Domain.Interfaces;
using Mediator;
using Moq;

namespace BugStore.Application.Tests.Product.Handlers;

public class DeleteProductHandlerTests
{
    private readonly Mock<IProductWriteRepository> _writeRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly DeleteProductHandler _handler;

    public DeleteProductHandlerTests()
    {
        _writeRepositoryMock = new Mock<IProductWriteRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _handler = new DeleteProductHandler(_writeRepositoryMock.Object, _unitOfWorkMock.Object);
    }

    [Fact]
    public async Task Handle_WithValidId_ShouldDeleteProductAndReturnUnit()
    {
        // Arrange
        var productId = Guid.CreateVersion7();
        var command = new DeleteProductCommand(productId);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.Equal(Unit.Value, result);

        _writeRepositoryMock.Verify(x => x.DeleteAsync(productId), Times.Once);
        _unitOfWorkMock.Verify(x => x.CommitAsync(), Times.Once);
    }

    [Fact]
    public async Task Handle_WithAnyId_ShouldCallDeleteAndCommit()
    {
        // Arrange
        var productId = Guid.CreateVersion7();
        var command = new DeleteProductCommand(productId);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _writeRepositoryMock.Verify(x => x.DeleteAsync(productId), Times.Once);
        _unitOfWorkMock.Verify(x => x.CommitAsync(), Times.Once);
    }
}
