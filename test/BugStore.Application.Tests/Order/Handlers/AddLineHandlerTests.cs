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
    private readonly Mock<IProductReadOnlyRepository> _productRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IOrderLineValidator> _validatorMock;
    private readonly AddLineHandler _handler;

    public AddLineHandlerTests()
    {
        _writeRepositoryMock = new Mock<IOrderWriteRepository>();
        _readRepositoryMock = new Mock<IOrderReadOnlyRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _validatorMock = new Mock<IOrderLineValidator>();
        _productRepositoryMock = new Mock<IProductReadOnlyRepository>();
        _handler = new AddLineHandler(
            _writeRepositoryMock.Object,
            _readRepositoryMock.Object,
            _unitOfWorkMock.Object,
            _validatorMock.Object,
            _productRepositoryMock.Object
        );
    }

    [Fact]
    public async Task Handle_WithValidRequest_ShouldAddLineAndReturnResponse()
    {
        // Arrange
        var orderId = Guid.CreateVersion7();
        var productId = Guid.CreateVersion7();
        var customerId = Guid.CreateVersion7();
        var quantity = 3;
        var price = 75.50m;
        
        var existingOrder = OrderBuilder.Build(customerId);
        var product = ProductBuilder.Build(price: price);
        var request = OrderDTOBuilder.BuildLineRequest(productId, quantity, price);
        var command = new AddLineCommand(orderId, request);

        // Setup validation to pass
        _validatorMock
            .Setup(x => x.ValidateAsync(request, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new FluentValidation.Results.ValidationResult());

        // Setup order repository to return existing order
        _readRepositoryMock
            .Setup(x => x.GetByIdAsync(orderId))
            .ReturnsAsync(existingOrder);

        // Setup product repository to return existing product
        _productRepositoryMock
            .Setup(x => x.GetByIdAsync(productId))
            .ReturnsAsync(product);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(existingOrder.Id, result.Id);
        Assert.Equal(existingOrder.CustomerId, result.CustomerId);
        Assert.Equal(existingOrder.CreatedAt, result.CreatedAt);
        Assert.NotNull(result.UpdatedAt);
        Assert.Single(result.Lines);
        
        var addedLine = result.Lines.First();
        Assert.Equal(productId, addedLine.ProductId);
        Assert.Equal(quantity, addedLine.Quantity);
        Assert.Equal(price, addedLine.Price);
        Assert.Equal(quantity * price, addedLine.Total);
        Assert.Equal(quantity * price, result.Total);

        // Verify all repository calls were made
        _validatorMock.Verify(x => x.ValidateAsync(request, It.IsAny<CancellationToken>()), Times.Once);
        _readRepositoryMock.Verify(x => x.GetByIdAsync(orderId), Times.Once);
        _productRepositoryMock.Verify(x => x.GetByIdAsync(productId), Times.Once);
        _writeRepositoryMock.Verify(x => x.AddLineAsync(It.IsAny<Domain.Entities.OrderLine>()), Times.Once);
        _writeRepositoryMock.Verify(x => x.UpdateAsync(existingOrder), Times.Once);
        _unitOfWorkMock.Verify(x => x.CommitAsync(), Times.Once);
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
