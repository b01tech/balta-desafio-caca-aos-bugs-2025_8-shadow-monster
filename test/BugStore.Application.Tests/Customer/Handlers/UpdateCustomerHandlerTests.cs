using BugStore.Application.Customer.Commands;
using BugStore.Application.Customer.Handlers;
using BugStore.Domain.Interfaces;
using BugStore.Exception.ProjectException;
using BugStore.TestUtilities.Builders;
using Moq;

namespace BugStore.Application.Tests.Customer.Handlers;

public class UpdateCustomerHandlerTests
{
    private readonly Mock<ICustomerReadOnlyRepository> _readRepositoryMock;
    private readonly Mock<ICustomerWriteRepository> _writeRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly UpdateCustomerHadler _handler;

    public UpdateCustomerHandlerTests()
    {
        _readRepositoryMock = new Mock<ICustomerReadOnlyRepository>();
        _writeRepositoryMock = new Mock<ICustomerWriteRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _handler = new UpdateCustomerHadler(
            _writeRepositoryMock.Object,
            _readRepositoryMock.Object,
            _unitOfWorkMock.Object
        );
    }

    [Fact]
    public async Task Handle_WithValidRequest_ShouldUpdateCustomerAndReturnResponse()
    {
        // Arrange
        var customerId = Guid.CreateVersion7();
        var existingCustomer = CustomerBuilder.Build();
        var request = CustomerDTOBuilder.BuildRequest(
            name: "Updated Name",
            email: "updated@example.com"
        );
        var command = new UpdateCustomerCommand(customerId, request);

        _readRepositoryMock.Setup(x => x.GetByIdAsync(customerId)).ReturnsAsync(existingCustomer);

        _readRepositoryMock.Setup(x => x.ExistsByEmailAsync(request.Email)).ReturnsAsync(false);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(request.Name, result.Name);
        Assert.Equal(request.Email, result.Email);
        Assert.Equal(request.Phone, result.Phone);
        Assert.Equal(request.BirthDate, result.BirthDate);

        _writeRepositoryMock.Verify(x => x.UpdateAsync(existingCustomer), Times.Once);
        _unitOfWorkMock.Verify(x => x.CommitAsync(), Times.Once);
    }

    [Fact]
    public async Task Handle_WithNonExistentCustomer_ShouldThrowNotFoundException()
    {
        // Arrange
        var customerId = Guid.CreateVersion7();
        var request = CustomerDTOBuilder.BuildRequest();
        var command = new UpdateCustomerCommand(customerId, request);

        _readRepositoryMock
            .Setup(x => x.GetByIdAsync(customerId))
            .ReturnsAsync((Domain.Entities.Customer?)null);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(
            () => _handler.Handle(command, CancellationToken.None).AsTask()
        );

        _writeRepositoryMock.Verify(
            x => x.UpdateAsync(It.IsAny<Domain.Entities.Customer>()),
            Times.Never
        );
        _unitOfWorkMock.Verify(x => x.CommitAsync(), Times.Never);
    }

    [Fact]
    public async Task Handle_WithInvalidData_ShouldThrowOnValidationException()
    {
        // Arrange
        var customerId = Guid.CreateVersion7();
        var request = CustomerDTOBuilder.BuildRequest(name: "");
        var command = new UpdateCustomerCommand(customerId, request);

        // Act & Assert
        await Assert.ThrowsAsync<OnValidationException>(
            () => _handler.Handle(command, CancellationToken.None).AsTask()
        );

        _writeRepositoryMock.Verify(
            x => x.UpdateAsync(It.IsAny<Domain.Entities.Customer>()),
            Times.Never
        );
        _unitOfWorkMock.Verify(x => x.CommitAsync(), Times.Never);
    }

    [Fact]
    public async Task Handle_WithExistingEmail_ShouldThrowOnValidationException()
    {
        // Arrange
        var customerId = Guid.CreateVersion7();
        var existingCustomer = CustomerBuilder.Build();
        var request = CustomerDTOBuilder.BuildRequest(email: "existing@example.com");
        var command = new UpdateCustomerCommand(customerId, request);

        _readRepositoryMock.Setup(x => x.GetByIdAsync(customerId)).ReturnsAsync(existingCustomer);

        _readRepositoryMock.Setup(x => x.ExistsByEmailAsync(request.Email)).ReturnsAsync(true);

        // Act & Assert
        await Assert.ThrowsAsync<OnValidationException>(
            () => _handler.Handle(command, CancellationToken.None).AsTask()
        );

        _writeRepositoryMock.Verify(
            x => x.UpdateAsync(It.IsAny<Domain.Entities.Customer>()),
            Times.Never
        );
        _unitOfWorkMock.Verify(x => x.CommitAsync(), Times.Never);
    }
}
