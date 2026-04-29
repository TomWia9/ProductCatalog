using Application.Commands;
using Application.Interfaces;
using Domain.Entities;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;

namespace ProductCatalog.UnitTests.Commands;

/// <summary>
/// Unit tests for <see cref="CreateProductCommandHandler"/>.
/// </summary>
public sealed class CreateProductCommandHandlerTests
{
    private readonly IProductRepository _repositoryMock;
    private readonly CreateProductCommandHandler _handler;

    public CreateProductCommandHandlerTests()
    {
        _repositoryMock = Substitute.For<IProductRepository>();
        _handler = new CreateProductCommandHandler(
            _repositoryMock,
            NullLogger<CreateProductCommandHandler>.Instance);
    }

    [Fact]
    public async Task Handle_ValidCommand_ReturnsCreatedProductDto()
    {
        // Arrange
        var command = new CreateProductCommand("CODE-NEW", "New Product", 149.99m);

        _repositoryMock
            .ExistsByCodeAsync(command.Code, Arg.Any<CancellationToken>())
            .Returns(false);

        _repositoryMock
            .AddAsync(Arg.Any<Product>(), Arg.Any<CancellationToken>())
            .Returns(callInfo => Task.FromResult(callInfo.Arg<Product>()));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Code.Should().Be(command.Code);
        result.Name.Should().Be(command.Name);
        result.Price.Should().Be(command.Price);
        result.Id.Should().NotBeEmpty();
    }

    [Fact]
    public async Task Handle_DuplicateCode_ThrowsInvalidOperationException()
    {
        // Arrange
        var command = new CreateProductCommand("EXISTING-CODE", "Some Product", 50.00m);

        _repositoryMock
            .ExistsByCodeAsync(command.Code, Arg.Any<CancellationToken>())
            .Returns(true);

        // Act
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*EXISTING-CODE*");
    }

    [Fact]
    public async Task Handle_ValidCommand_ChecksCodeExistenceBeforeAdding()
    {
        // Arrange
        var command = new CreateProductCommand("UNIQUE-CODE", "Product", 10.00m);

        _repositoryMock
            .ExistsByCodeAsync(command.Code, Arg.Any<CancellationToken>())
            .Returns(false);

        _repositoryMock
            .AddAsync(Arg.Any<Product>(), Arg.Any<CancellationToken>())
            .Returns(callInfo => Task.FromResult(callInfo.Arg<Product>()));

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert - both were called exactly once
        await _repositoryMock.Received(1).ExistsByCodeAsync(command.Code, Arg.Any<CancellationToken>());
        await _repositoryMock.Received(1).AddAsync(Arg.Any<Product>(), Arg.Any<CancellationToken>());
    }
}