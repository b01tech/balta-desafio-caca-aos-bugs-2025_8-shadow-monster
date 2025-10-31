using BugStore.Application.Customer.Commands;
using BugStore.Application.Customer.Handlers;
using BugStore.Domain.Interfaces;
using BugStore.Exception.ProjectException;
using BugStore.TestUtilities.Builders;
using Mediator;
using Moq;

namespace BugStore.Application.Tests.Customer.Handlers;

public class DeleteCustomerHandlerTests
{
    private readonly Mock<ICustomerReadOnlyRepository> _readRepositoryMock;
    private readonly Mock<ICustomerWriteRepository> _writeRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly DeleteCustomerHandler _handler;

    public DeleteCustomerHandlerTests()
    {
        _readRepositoryMock = new Mock<ICustomerReadOnlyRepository>();
        _writeRepositoryMock = new Mock<ICustomerWriteRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _handler = new DeleteCustomerHandler(
            _writeRepositoryMock.Object,
            _readRepositoryMock.Object,
            _unitOfWorkMock.Object
        );
    }

    [Fact]
    public async Task Handle_WithExistingCustomer_ShouldDeleteCustomer()
    {
        // Arrange
        var customerId = Guid.CreateVersion7();
        var existingCustomer = CustomerBuilder.Build();
        var command = new DeleteCustomerCommand(customerId);

        _readRepositoryMock.Setup(x => x.GetByIdAsync(customerId)).ReturnsAsync(existingCustomer);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.Equal(Unit.Value, result);
        _writeRepositoryMock.Verify(x => x.DeleteAsync(customerId), Times.Once);
        _unitOfWorkMock.Verify(x => x.CommitAsync(), Times.Once);
    }

    [Fact]
    public async Task Handle_WithNonExistentCustomer_ShouldThrowNotFoundException()
    {
        // Arrange
        var customerId = Guid.CreateVersion7();
        var command = new DeleteCustomerCommand(customerId);

        _readRepositoryMock
            .Setup(x => x.GetByIdAsync(customerId))
            .ReturnsAsync((Domain.Entities.Customer?)null);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(
            () => _handler.Handle(command, CancellationToken.None).AsTask()
        );

        _writeRepositoryMock.Verify(x => x.DeleteAsync(It.IsAny<Guid>()), Times.Never);
        _unitOfWorkMock.Verify(x => x.CommitAsync(), Times.Never);
    }
}