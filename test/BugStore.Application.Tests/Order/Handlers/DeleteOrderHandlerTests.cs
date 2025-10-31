using BugStore.Application.Order.Commands;
using BugStore.Application.Order.Handlers;
using BugStore.Domain.Interfaces;
using Mediator;
using Moq;

namespace BugStore.Application.Tests.Order.Handlers;

public class DeleteOrderHandlerTests
{
    private readonly Mock<IOrderWriteRepository> _writeRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly DeleteOrderHandler _handler;

    public DeleteOrderHandlerTests()
    {
        _writeRepositoryMock = new Mock<IOrderWriteRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _handler = new DeleteOrderHandler(_writeRepositoryMock.Object, _unitOfWorkMock.Object);
    }

    [Fact]
    public async Task Handle_WithValidId_ShouldDeleteOrderAndReturnUnit()
    {
        // Arrange
        var orderId = Guid.CreateVersion7();
        var command = new DeleteOrderCommand(orderId);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.Equal(Unit.Value, result);

        _writeRepositoryMock.Verify(x => x.DeleteAsync(orderId), Times.Once);
        _unitOfWorkMock.Verify(x => x.CommitAsync(), Times.Once);
    }

    [Fact]
    public async Task Handle_WithAnyId_ShouldCallDeleteAndCommit()
    {
        // Arrange
        var orderId = Guid.CreateVersion7();
        var command = new DeleteOrderCommand(orderId);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _writeRepositoryMock.Verify(x => x.DeleteAsync(orderId), Times.Once);
        _unitOfWorkMock.Verify(x => x.CommitAsync(), Times.Once);
    }
}
