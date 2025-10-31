using BugStore.Application.Order.Commands;
using BugStore.Application.Order.Handlers;
using BugStore.Application.Order.Validators;
using BugStore.Domain.Interfaces;
using BugStore.Exception.ProjectException;
using BugStore.TestUtilities.Builders;
using Moq;

namespace BugStore.Application.Tests.Order.Handlers;

public class AddLineHandlerTests
{
    private readonly Mock<IOrderWriteRepository> _writeRepositoryMock;
    private readonly Mock<IOrderReadOnlyRepository> _readRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IOrderLineValidator> _validatorMock;
    private readonly AddLineHandler _handler;

    public AddLineHandlerTests()
    {
        _writeRepositoryMock = new Mock<IOrderWriteRepository>();
        _readRepositoryMock = new Mock<IOrderReadOnlyRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _validatorMock = new Mock<IOrderLineValidator>();
        _handler = new AddLineHandler(
            _writeRepositoryMock.Object,
            _readRepositoryMock.Object,
            _unitOfWorkMock.Object,
            _validatorMock.Object
        );
    }

    [Fact]
    public async Task Handle_WithValidRequest_ShouldAddLineAndReturnResponse()
    {
        // Arrange
        var orderId = Guid.CreateVersion7();
        var existingOrder = OrderBuilder.Build();
        var updatedOrder = OrderBuilder.BuildWithLines();
        
        // Populate Product properties for the updated order lines
        foreach (var line in updatedOrder.Lines)
        {
            line.Product = ProductBuilder.Build(price: 50.00m);
        }
        
        var request = OrderDTOBuilder.BuildLineRequest();
        var command = new AddLineCommand(orderId, request);

        _validatorMock
            .Setup(x => x.ValidateAsync(request, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new FluentValidation.Results.ValidationResult());

        _readRepositoryMock
            .SetupSequence(x => x.GetByIdAsync(orderId))
            .ReturnsAsync(existingOrder)
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
        Assert.NotEmpty(result.Lines);

        _writeRepositoryMock.Verify(x => x.UpdateAsync(existingOrder), Times.Once);
        _unitOfWorkMock.Verify(x => x.CommitAsync(), Times.Once);
        _readRepositoryMock.Verify(x => x.GetByIdAsync(orderId), Times.Exactly(2));
    }

    [Fact]
    public async Task Handle_WithNonExistentOrder_ShouldThrowNotFoundException()
    {
        // Arrange
        var orderId = Guid.CreateVersion7();
        var request = OrderDTOBuilder.BuildLineRequest();
        var command = new AddLineCommand(orderId, request);

        _validatorMock
            .Setup(x => x.ValidateAsync(request, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new FluentValidation.Results.ValidationResult());

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
    public async Task Handle_WithInvalidRequest_ShouldThrowOnValidationException()
    {
        // Arrange
        var orderId = Guid.CreateVersion7();
        var request = OrderDTOBuilder.BuildLineRequest();
        var command = new AddLineCommand(orderId, request);
        var validationErrors = new List<FluentValidation.Results.ValidationFailure>
        {
            new("ProductId", "Product ID is required")
        };
        var validationResult = new FluentValidation.Results.ValidationResult(validationErrors);

        _validatorMock
            .Setup(x => x.ValidateAsync(request, It.IsAny<CancellationToken>()))
            .ReturnsAsync(validationResult);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<OnValidationException>(
            () => _handler.Handle(command, CancellationToken.None).AsTask()
        );

        Assert.NotNull(exception.ErrorMessages);

        _readRepositoryMock.Verify(x => x.GetByIdAsync(It.IsAny<Guid>()), Times.Never);
        _writeRepositoryMock.Verify(
            x => x.UpdateAsync(It.IsAny<Domain.Entities.Order>()),
            Times.Never
        );
        _unitOfWorkMock.Verify(x => x.CommitAsync(), Times.Never);
    }
}
