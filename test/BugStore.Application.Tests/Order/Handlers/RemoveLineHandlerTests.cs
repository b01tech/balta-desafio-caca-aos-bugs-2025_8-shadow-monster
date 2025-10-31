using BugStore.Application.Order.Commands;
using BugStore.Application.Order.Handlers;
using BugStore.Domain.Interfaces;
using BugStore.Exception.ProjectException;
using BugStore.TestUtilities.Builders;
using Moq;

namespace BugStore.Application.Tests.Order.Handlers;

public class RemoveLineHandlerTests
{
    private readonly Mock<IOrderWriteRepository> _writeRepositoryMock;
    private readonly Mock<IOrderReadOnlyRepository> _readRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly RemoveLineHandler _handler;

    public RemoveLineHandlerTests()
    {
        _writeRepositoryMock = new Mock<IOrderWriteRepository>();
        _readRepositoryMock = new Mock<IOrderReadOnlyRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _handler = new RemoveLineHandler(
            _writeRepositoryMock.Object,
            _readRepositoryMock.Object,
            _unitOfWorkMock.Object
        );
    }

    [Fact]
    public async Task Handle_WithValidRequest_ShouldRemoveLineAndReturnResponse()
    {
        // Arrange
        var orderId = Guid.CreateVersion7();
        var productId = Guid.CreateVersion7();
        var orderWithLines = OrderBuilder.BuildWithLines();
        var updatedOrder = OrderBuilder.Build();
        var command = new RemoveLineCommand(orderId, productId);

        // Add a line with the specific productId to the order
        orderWithLines.AddLine(productId, 2, 50.00m);

        _readRepositoryMock
            .SetupSequence(x => x.GetByIdAsync(orderId))
            .ReturnsAsync(orderWithLines)
            .ReturnsAsync(updatedOrder);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(updatedOrder.Id, result.Id);
        Assert.Equal(updatedOrder.CustomerId, result.CustomerId);
        Assert.Equal(updatedOrder.CreatedAt, result.CreatedAt);
        Assert.Equal(updatedOrder.UpdatedAt, result.UpdatedAt);
        Assert.Equal(updatedOrder.Total, result.Total);

        _writeRepositoryMock.Verify(x => x.UpdateAsync(orderWithLines), Times.Once);
        _unitOfWorkMock.Verify(x => x.CommitAsync(), Times.Once);
        _readRepositoryMock.Verify(x => x.GetByIdAsync(orderId), Times.Exactly(2));
    }

    [Fact]
    public async Task Handle_WithNonExistentOrder_ShouldThrowNotFoundException()
    {
        // Arrange
        var orderId = Guid.CreateVersion7();
        var productId = Guid.CreateVersion7();
        var command = new RemoveLineCommand(orderId, productId);

        _readRepositoryMock
            .Setup(x => x.GetByIdAsync(orderId))
            .ReturnsAsync((Domain.Entities.Order?)null);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(
            () => _handler.Handle(command, CancellationToken.None).AsTask()
        );

        _writeRepositoryMock.Verify(
            x => x.UpdateAsync(It.IsAny<Domain.Entities.Order>()),
            Times.Never
        );
        _unitOfWorkMock.Verify(x => x.CommitAsync(), Times.Never);
    }

    [Fact]
    public async Task Handle_WithNonExistentProductInOrder_ShouldThrowNotFoundException()
    {
        // Arrange
        var orderId = Guid.CreateVersion7();
        var productId = Guid.CreateVersion7();
        var orderWithoutProduct = OrderBuilder.Build(); // Order without the specific product
        var command = new RemoveLineCommand(orderId, productId);

        _readRepositoryMock.Setup(x => x.GetByIdAsync(orderId)).ReturnsAsync(orderWithoutProduct);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(
            () => _handler.Handle(command, CancellationToken.None).AsTask()
        );

        _writeRepositoryMock.Verify(
            x => x.UpdateAsync(It.IsAny<Domain.Entities.Order>()),
            Times.Never
        );
        _unitOfWorkMock.Verify(x => x.CommitAsync(), Times.Never);
    }
}
