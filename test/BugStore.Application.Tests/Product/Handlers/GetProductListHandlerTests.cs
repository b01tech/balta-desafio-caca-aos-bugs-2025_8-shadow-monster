using BugStore.Application.Product.Handlers;
using BugStore.Application.Product.Queries;
using BugStore.Domain.Interfaces;
using BugStore.TestUtilities.Builders;
using Moq;

namespace BugStore.Application.Tests.Product.Handlers;

public class GetProductListHandlerTests
{
    private readonly Mock<IProductReadOnlyRepository> _repositoryMock;
    private readonly GetProductListHandler _handler;

    public GetProductListHandlerTests()
    {
        _repositoryMock = new Mock<IProductReadOnlyRepository>();
        _handler = new GetProductListHandler(_repositoryMock.Object);
    }

    [Fact]
    public async Task Handle_WithValidRequest_ShouldReturnPaginatedProducts()
    {
        // Arrange
        var page = 1;
        var pageSize = 10;
        var totalProducts = 25L;
        var products = new List<Domain.Entities.Product>
        {
            ProductBuilder.Build(),
            ProductBuilder.Build(),
            ProductBuilder.Build()
        };
        var query = new GetProductListQuery(page, pageSize);

        _repositoryMock.Setup(x => x.GetAllAsync(page, pageSize)).ReturnsAsync(products);

        _repositoryMock.Setup(x => x.GetTotalItemAsync()).ReturnsAsync(totalProducts);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(totalProducts, result.TotalItems);
        Assert.Equal(page, result.Page);
        Assert.Equal(3, result.TotalPages); // 25 / 10 = 3 pages
        Assert.Equal(products.Count, result.Products.Count);

        for (int i = 0; i < products.Count; i++)
        {
            Assert.Equal(products[i].Id, result.Products[i].Id);
            Assert.Equal(products[i].Title, result.Products[i].Title);
            Assert.Equal(products[i].Price, result.Products[i].Price);
        }
    }

    [Fact]
    public async Task Handle_WithEmptyResult_ShouldReturnEmptyList()
    {
        // Arrange
        var page = 1;
        var pageSize = 10;
        var totalProducts = 0L;
        var products = new List<Domain.Entities.Product>();
        var query = new GetProductListQuery(page, pageSize);

        _repositoryMock.Setup(x => x.GetAllAsync(page, pageSize)).ReturnsAsync(products);

        _repositoryMock.Setup(x => x.GetTotalItemAsync()).ReturnsAsync(totalProducts);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(0, result.TotalItems);
        Assert.Equal(page, result.Page);
        Assert.Equal(0, result.TotalPages);
        Assert.Empty(result.Products);
    }

    [Fact]
    public async Task Handle_WithDifferentPageSize_ShouldCalculateCorrectTotalPages()
    {
        // Arrange
        var page = 2;
        var pageSize = 5;
        var totalProducts = 12L;
        var products = new List<Domain.Entities.Product>
        {
            ProductBuilder.Build(),
            ProductBuilder.Build()
        };
        var query = new GetProductListQuery(page, pageSize);

        _repositoryMock.Setup(x => x.GetAllAsync(page, pageSize)).ReturnsAsync(products);

        _repositoryMock.Setup(x => x.GetTotalItemAsync()).ReturnsAsync(totalProducts);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(totalProducts, result.TotalItems);
        Assert.Equal(page, result.Page);
        Assert.Equal(3, result.TotalPages); // 12 / 5 = 3 pages (rounded up)
        Assert.Equal(products.Count, result.Products.Count);
    }
}
