using BugStore.Application.Customer.Handlers;
using BugStore.Application.Customer.Queries;
using BugStore.Domain.Interfaces;
using BugStore.TestUtilities.Builders;
using Moq;

namespace BugStore.Application.Tests.Customer.Handlers;

public class GetCustomerListHandlerTests
{
    private readonly Mock<ICustomerReadOnlyRepository> _repositoryMock;
    private readonly GetCustomerListHandler _handler;

    public GetCustomerListHandlerTests()
    {
        _repositoryMock = new Mock<ICustomerReadOnlyRepository>();
        _handler = new GetCustomerListHandler(_repositoryMock.Object);
    }

    [Fact]
    public async Task Handle_WithValidRequest_ShouldReturnPaginatedCustomers()
    {
        // Arrange
        var page = 1;
        var pageSize = 10;
        var totalCustomers = 25L;
        var customers = new List<Domain.Entities.Customer>
        {
            CustomerBuilder.Build(),
            CustomerBuilder.Build(),
            CustomerBuilder.Build()
        };
        var query = new GetCustomerListQuery(page, pageSize);

        _repositoryMock
            .Setup(x => x.GetAllAsync(page, pageSize))
            .ReturnsAsync(customers);

        _repositoryMock
            .Setup(x => x.GetTotalItemAsync())
            .ReturnsAsync(totalCustomers);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(totalCustomers, result.TotalCustomers);
        Assert.Equal(page, result.Page);
        Assert.Equal(3, result.TotalPages); // 25 / 10 = 3 pages
        Assert.Equal(customers.Count, result.Customers.Count);

        for (int i = 0; i < customers.Count; i++)
        {
            Assert.Equal(customers[i].Id, result.Customers[i].Id);
            Assert.Equal(customers[i].Name, result.Customers[i].Name);
            Assert.Equal(customers[i].Email, result.Customers[i].Email);
            Assert.Equal(customers[i].Phone, result.Customers[i].Phone);
            Assert.Equal(customers[i].BirthDate, result.Customers[i].BirthDate);
        }
    }

    [Fact]
    public async Task Handle_WithEmptyResult_ShouldReturnEmptyList()
    {
        // Arrange
        var page = 1;
        var pageSize = 10;
        var totalCustomers = 0L;
        var customers = new List<Domain.Entities.Customer>();
        var query = new GetCustomerListQuery(page, pageSize);

        _repositoryMock
            .Setup(x => x.GetAllAsync(page, pageSize))
            .ReturnsAsync(customers);

        _repositoryMock
            .Setup(x => x.GetTotalItemAsync())
            .ReturnsAsync(totalCustomers);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(0, result.TotalCustomers);
        Assert.Equal(page, result.Page);
        Assert.Equal(0, result.TotalPages);
        Assert.Empty(result.Customers);
    }

    [Fact]
    public async Task Handle_WithDifferentPageSize_ShouldCalculateCorrectTotalPages()
    {
        // Arrange
        var page = 2;
        var pageSize = 5;
        var totalCustomers = 12L;
        var customers = new List<Domain.Entities.Customer>
        {
            CustomerBuilder.Build(),
            CustomerBuilder.Build()
        };
        var query = new GetCustomerListQuery(page, pageSize);

        _repositoryMock
            .Setup(x => x.GetAllAsync(page, pageSize))
            .ReturnsAsync(customers);

        _repositoryMock
            .Setup(x => x.GetTotalItemAsync())
            .ReturnsAsync(totalCustomers);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(totalCustomers, result.TotalCustomers);
        Assert.Equal(page, result.Page);
        Assert.Equal(3, result.TotalPages); // 12 / 5 = 3 pages (rounded up)
        Assert.Equal(customers.Count, result.Customers.Count);
    }
}
