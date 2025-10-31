using BugStore.Application.Customer.Commands;
using BugStore.Application.Customer.Handlers;
using BugStore.Domain.Interfaces;
using BugStore.Exception.ProjectException;
using BugStore.TestUtilities.Builders;
using Moq;

namespace BugStore.Application.Tests.Customer.Handlers;

public class AddCustomerHandlerTests
{
    private readonly Mock<ICustomerReadOnlyRepository> _readRepositoryMock;
    private readonly Mock<ICustomerWriteRepository> _writeRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly AddCustomerHandler _handler;

    public AddCustomerHandlerTests()
    {
        _readRepositoryMock = new Mock<ICustomerReadOnlyRepository>();
        _writeRepositoryMock = new Mock<ICustomerWriteRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _handler = new AddCustomerHandler(
            _readRepositoryMock.Object,
            _writeRepositoryMock.Object,
            _unitOfWorkMock.Object
        );
    }

    [Fact]
    public async Task Handle_WithValidRequest_ShouldAddCustomerAndReturnResponse()
    {
        // Arrange
        var request = CustomerDTOBuilder.BuildRequest();
        var command = new AddCustomerCommand(request);

        _readRepositoryMock
            .Setup(x => x.ExistsByEmailAsync(request.Email))
            .ReturnsAsync(false);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(request.Name, result.Name);
        Assert.Equal(request.Email, result.Email);
        Assert.Equal(request.Phone, result.Phone);
        Assert.Equal(request.BirthDate, result.BirthDate);

        _writeRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Domain.Entities.Customer>()), Times.Once);
        _unitOfWorkMock.Verify(x => x.CommitAsync(), Times.Once);
    }

    [Fact]
    public async Task Handle_WithEmptyName_ShouldThrowOnValidationException()
    {
        // Arrange
        var request = CustomerDTOBuilder.BuildRequest(name: "");
        var command = new AddCustomerCommand(request);

        // Act & Assert
        await Assert.ThrowsAsync<OnValidationException>(() => 
            _handler.Handle(command, CancellationToken.None).AsTask());

        _writeRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Domain.Entities.Customer>()), Times.Never);
        _unitOfWorkMock.Verify(x => x.CommitAsync(), Times.Never);
    }

    [Fact]
    public async Task Handle_WithInvalidEmail_ShouldThrowOnValidationException()
    {
        // Arrange
        var request = CustomerDTOBuilder.BuildRequest(email: "invalid-email");
        var command = new AddCustomerCommand(request);

        // Act & Assert
        await Assert.ThrowsAsync<OnValidationException>(() => 
            _handler.Handle(command, CancellationToken.None).AsTask());

        _writeRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Domain.Entities.Customer>()), Times.Never);
        _unitOfWorkMock.Verify(x => x.CommitAsync(), Times.Never);
    }

    [Fact]
    public async Task Handle_WithFutureBirthDate_ShouldThrowOnValidationException()
    {
        // Arrange
        var request = CustomerDTOBuilder.BuildRequest(birthDate: DateTime.Now.AddDays(1));
        var command = new AddCustomerCommand(request);

        // Act & Assert
        await Assert.ThrowsAsync<OnValidationException>(() => 
            _handler.Handle(command, CancellationToken.None).AsTask());

        _writeRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Domain.Entities.Customer>()), Times.Never);
        _unitOfWorkMock.Verify(x => x.CommitAsync(), Times.Never);
    }

    [Fact]
    public async Task Handle_WithExistingEmail_ShouldThrowOnValidationException()
    {
        // Arrange
        var request = CustomerDTOBuilder.BuildRequest();
        var command = new AddCustomerCommand(request);

        _readRepositoryMock
            .Setup(x => x.ExistsByEmailAsync(request.Email))
            .ReturnsAsync(true);

        // Act & Assert
        await Assert.ThrowsAsync<OnValidationException>(() => 
            _handler.Handle(command, CancellationToken.None).AsTask());

        _writeRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Domain.Entities.Customer>()), Times.Never);
        _unitOfWorkMock.Verify(x => x.CommitAsync(), Times.Never);
    }
}
