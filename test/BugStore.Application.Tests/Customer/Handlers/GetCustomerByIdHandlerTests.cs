using BugStore.Application.Customer.Handlers;
using BugStore.Application.Customer.Queries;
using BugStore.Domain.Interfaces;
using BugStore.Exception.ProjectException;
using BugStore.TestUtilities.Builders;
using Moq;

namespace BugStore.Application.Tests.Customer.Handlers;

public class GetCustomerByIdHandlerTests
{
    private readonly Mock<ICustomerReadOnlyRepository> _repositoryMock;
    private readonly GetCustomerByIdHandler _handler;

    public GetCustomerByIdHandlerTests()
    {
        _repositoryMock = new Mock<ICustomerReadOnlyRepository>();
        _handler = new GetCustomerByIdHandler(_repositoryMock.Object);
    }

    [Fact]
    public async Task Handle_WithExistingCustomer_ShouldReturnCustomerData()
    {
        // Arrange
        var customerId = Guid.CreateVersion7();
        var existingCustomer = CustomerBuilder.Build();
        var query = new GetCustomerByIdQuery(customerId);

        _repositoryMock.Setup(x => x.GetByIdAsync(customerId)).ReturnsAsync(existingCustomer);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(existingCustomer.Id, result.Id);
        Assert.Equal(existingCustomer.Name, result.Name);
        Assert.Equal(existingCustomer.Email, result.Email);
        Assert.Equal(existingCustomer.Phone, result.Phone);
        Assert.Equal(existingCustomer.BirthDate, result.BirthDate);
    }

    [Fact]
    public async Task Handle_WithNonExistentCustomer_ShouldThrowNotFoundException()
    {
        // Arrange
        var customerId = Guid.CreateVersion7();
        var query = new GetCustomerByIdQuery(customerId);

        _repositoryMock
            .Setup(x => x.GetByIdAsync(customerId))
            .ReturnsAsync((Domain.Entities.Customer?)null);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(
            () => _handler.Handle(query, CancellationToken.None).AsTask()
        );
    }
}