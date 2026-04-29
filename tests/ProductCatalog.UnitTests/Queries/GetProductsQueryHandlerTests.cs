using Application.Interfaces;
using Application.Queries;
using Domain.Entities;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;

namespace ProductCatalog.UnitTests.Queries;

/// <summary>
/// Unit tests for <see cref="GetProductsQueryHandler"/>.
/// </summary>
public sealed class GetProductsQueryHandlerTests
{
    private readonly IProductRepository _repositoryMock;
    private readonly GetProductsQueryHandler _handler;

    public GetProductsQueryHandlerTests()
    {
        _repositoryMock = Substitute.For<IProductRepository>();
        _handler = new GetProductsQueryHandler(
            _repositoryMock,
            NullLogger<GetProductsQueryHandler>.Instance);
    }

    [Fact]
    public async Task Handle_WhenProductsExist_ReturnsMappedDtos()
    {
        // Arrange
        var products = new List<Product>
        {
            Product.Create("CODE-001", "Product One", 10.00m),
            Product.Create("CODE-002", "Product Two", 20.00m)
        };

        _repositoryMock
            .GetAllAsync(Arg.Any<CancellationToken>())
            .Returns(products.AsReadOnly());

        // Act
        var result = await _handler.Handle(new GetProductsQuery(), CancellationToken.None);

        // Assert
        result.Should().HaveCount(2);
        result.Should().ContainSingle(p => p.Code == "CODE-001" && p.Name == "Product One" && p.Price == 10.00m);
        result.Should().ContainSingle(p => p.Code == "CODE-002" && p.Name == "Product Two" && p.Price == 20.00m);
    }

    [Fact]
    public async Task Handle_WhenNoProductsExist_ReturnsEmptyCollection()
    {
        // Arrange
        _repositoryMock
            .GetAllAsync(Arg.Any<CancellationToken>())
            .Returns(Array.Empty<Product>());

        // Act
        var result = await _handler.Handle(new GetProductsQuery(), CancellationToken.None);

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task Handle_CallsRepositoryOnce()
    {
        // Arrange
        _repositoryMock
            .GetAllAsync(Arg.Any<CancellationToken>())
            .Returns(Array.Empty<Product>());

        // Act
        await _handler.Handle(new GetProductsQuery(), CancellationToken.None);

        // Assert
        await _repositoryMock.Received(1).GetAllAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_MapsAllFieldsCorrectly()
    {
        // Arrange
        var product = Product.Create("SKU-XYZ", "Test Product", 99.99m);
        _repositoryMock
            .GetAllAsync(Arg.Any<CancellationToken>())
            .Returns(new List<Product> { product }.AsReadOnly());

        // Act
        var result = await _handler.Handle(new GetProductsQuery(), CancellationToken.None);

        // Assert
        var dto = result.Single();
        dto.Id.Should().Be(product.Id);
        dto.Code.Should().Be(product.Code);
        dto.Name.Should().Be(product.Name);
        dto.Price.Should().Be(product.Price);
        dto.CreatedAtUtc.Should().Be(product.CreatedAtUtc);
    }
}