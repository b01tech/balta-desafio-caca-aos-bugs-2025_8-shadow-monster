using BugStore.Application.Order.Commands;
using BugStore.Application.Order.Handlers;
using BugStore.Domain.Interfaces;
using BugStore.TestUtilities.Builders;
using Moq;

namespace BugStore.Application.Tests.Order.Handlers;

public class CreateOrderHandlerTests
{
    private readonly Mock<IOrderWriteRepository> _writeRepositoryMock;
    private readonly Mock<IOrderReadOnlyRepository> _readRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly CreateOrderHandler _handler;

    public CreateOrderHandlerTests()
    {
        _writeRepositoryMock = new Mock<IOrderWriteRepository>();
        _readRepositoryMock = new Mock<IOrderReadOnlyRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _handler = new CreateOrderHandler(
            _writeRepositoryMock.Object,
            _readRepositoryMock.Object,
            _unitOfWorkMock.Object
        );
    }

    [Fact]
    public async Task Handle_WithValidCustomerId_ShouldCreateOrderAndReturnResponse()
    {
        // Arrange
        var customerId = Guid.CreateVersion7();
        var createdOrder = OrderBuilder.Build();
        var command = new CreateOrderCommand(customerId);

        _readRepositoryMock.Setup(x => x.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(createdOrder);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(createdOrder.Id, result.Id);
        Assert.Equal(createdOrder.CustomerId, result.CustomerId);
        Assert.Equal(createdOrder.CreatedAt, result.CreatedAt);
        Assert.Equal(createdOrder.Total, result.Total);

        _writeRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Domain.Entities.Order>()), Times.Once);
        _unitOfWorkMock.Verify(x => x.CommitAsync(), Times.Once);
        _readRepositoryMock.Verify(x => x.GetByIdAsync(It.IsAny<Guid>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldCreateOrderWithCorrectCustomerId()
    {
        // Arrange
        var customerId = Guid.CreateVersion7();
        var createdOrder = OrderBuilder.Build(customerId);
        var command = new CreateOrderCommand(customerId);
        Domain.Entities.Order? capturedOrder = null;

        _writeRepositoryMock
            .Setup(x => x.AddAsync(It.IsAny<Domain.Entities.Order>()))
            .Callback<Domain.Entities.Order>(order => capturedOrder = order);

        _readRepositoryMock.Setup(x => x.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(createdOrder);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(capturedOrder);
        Assert.Equal(customerId, capturedOrder.CustomerId);
    }
}
