using BugStore.Application.Order.Handlers;
using BugStore.Application.Order.Queries;
using BugStore.Domain.Interfaces;
using BugStore.Exception.ProjectException;
using BugStore.TestUtilities.Builders;
using Moq;

namespace BugStore.Application.Tests.Order.Handlers;

public class GetOrderByIdHandlerTests
{
    private readonly Mock<IOrderReadOnlyRepository> _readRepositoryMock;
    private readonly GetOrderByIdHandler _handler;

    public GetOrderByIdHandlerTests()
    {
        _readRepositoryMock = new Mock<IOrderReadOnlyRepository>();
        _handler = new GetOrderByIdHandler(_readRepositoryMock.Object);
    }

    [Fact]
    public async Task Handle_WithExistingOrder_ShouldReturnOrderDetails()
    {
        // Arrange
        var orderId = Guid.CreateVersion7();
        var order = OrderBuilder.BuildWithLines();
        var query = new GetOrderByIdQuery(orderId);

        _readRepositoryMock.Setup(x => x.GetByIdAsync(orderId)).ReturnsAsync(order);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(order.Id, result.Id);
        Assert.Equal(order.CustomerId, result.CustomerId);
        Assert.Equal(order.CreatedAt, result.CreatedAt);
        Assert.Equal(order.UpdatedAt, result.UpdatedAt);
        Assert.Equal(order.Total, result.Total);
        Assert.NotNull(result.Lines);
        Assert.Equal(order.Lines.Count, result.Lines.Count);

        _readRepositoryMock.Verify(x => x.GetByIdAsync(orderId), Times.Once);
    }

    [Fact]
    public async Task Handle_WithNonExistentOrder_ShouldThrowNotFoundException()
    {
        // Arrange
        var orderId = Guid.CreateVersion7();
        var query = new GetOrderByIdQuery(orderId);

        _readRepositoryMock
            .Setup(x => x.GetByIdAsync(orderId))
            .ReturnsAsync((Domain.Entities.Order?)null);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(
            () => _handler.Handle(query, CancellationToken.None).AsTask()
        );

        _readRepositoryMock.Verify(x => x.GetByIdAsync(orderId), Times.Once);
    }
}
