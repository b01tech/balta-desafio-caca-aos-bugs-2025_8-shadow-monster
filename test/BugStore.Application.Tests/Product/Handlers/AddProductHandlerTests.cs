using BugStore.Application.Product.Commands;
using BugStore.Application.Product.Handlers;
using BugStore.Domain.Interfaces;
using BugStore.Exception.ProjectException;
using BugStore.TestUtilities.Builders;
using Moq;

namespace BugStore.Application.Tests.Product.Handlers;

public class AddProductHandlerTests
{
    private readonly Mock<IProductWriteRepository> _repositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly AddProductHandler _handler;

    public AddProductHandlerTests()
    {
        _repositoryMock = new Mock<IProductWriteRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _handler = new AddProductHandler(_repositoryMock.Object, _unitOfWorkMock.Object);
    }

    [Fact]
    public async Task Handle_WithValidRequest_ShouldAddProductAndReturnResponse()
    {
        // Arrange
        var request = ProductDTOBuilder.BuildRequest();
        var command = new AddProductCommand(request);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(request.Title, result.Title);
        Assert.Equal(request.Description, result.Description);
        Assert.Equal(request.Slug, result.Slug);
        Assert.Equal(request.Price, result.Price);

        _repositoryMock.Verify(x => x.AddAsync(It.IsAny<Domain.Entities.Product>()), Times.Once);
        _unitOfWorkMock.Verify(x => x.CommitAsync(), Times.Once);
    }

    [Fact]
    public async Task Handle_WithEmptyTitle_ShouldThrowOnValidationException()
    {
        // Arrange
        var request = ProductDTOBuilder.BuildRequest() with
        {
            Title = ""
        };
        var command = new AddProductCommand(request);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<OnValidationException>(
            () => _handler.Handle(command, CancellationToken.None).AsTask()
        );

        Assert.NotNull(exception.ErrorMessages);
        Assert.Contains(exception.ErrorMessages, e => e.Contains("Title"));

        _repositoryMock.Verify(x => x.AddAsync(It.IsAny<Domain.Entities.Product>()), Times.Never);
        _unitOfWorkMock.Verify(x => x.CommitAsync(), Times.Never);
    }

    [Fact]
    public async Task Handle_WithEmptyDescription_ShouldThrowOnValidationException()
    {
        // Arrange
        var request = ProductDTOBuilder.BuildRequest() with
        {
            Description = ""
        };
        var command = new AddProductCommand(request);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<OnValidationException>(
            () => _handler.Handle(command, CancellationToken.None).AsTask()
        );

        Assert.NotNull(exception.ErrorMessages);
        Assert.Contains(exception.ErrorMessages, e => e.Contains("Description"));

        _repositoryMock.Verify(x => x.AddAsync(It.IsAny<Domain.Entities.Product>()), Times.Never);
        _unitOfWorkMock.Verify(x => x.CommitAsync(), Times.Never);
    }

    [Fact]
    public async Task Handle_WithEmptySlug_ShouldThrowOnValidationException()
    {
        // Arrange
        var request = ProductDTOBuilder.BuildRequest() with
        {
            Slug = ""
        };
        var command = new AddProductCommand(request);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<OnValidationException>(
            () => _handler.Handle(command, CancellationToken.None).AsTask()
        );

        Assert.NotNull(exception.ErrorMessages);
        Assert.Contains(exception.ErrorMessages, e => e.Contains("Slug"));

        _repositoryMock.Verify(x => x.AddAsync(It.IsAny<Domain.Entities.Product>()), Times.Never);
        _unitOfWorkMock.Verify(x => x.CommitAsync(), Times.Never);
    }

    [Fact]
    public async Task Handle_WithNegativePrice_ShouldThrowOnValidationException()
    {
        // Arrange
        var request = ProductDTOBuilder.BuildRequest() with
        {
            Price = -10
        };
        var command = new AddProductCommand(request);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<OnValidationException>(
            () => _handler.Handle(command, CancellationToken.None).AsTask()
        );

        Assert.NotNull(exception.ErrorMessages);
        Assert.Contains(exception.ErrorMessages, e => e.Contains("Price"));

        _repositoryMock.Verify(x => x.AddAsync(It.IsAny<Domain.Entities.Product>()), Times.Never);
        _unitOfWorkMock.Verify(x => x.CommitAsync(), Times.Never);
    }
}
