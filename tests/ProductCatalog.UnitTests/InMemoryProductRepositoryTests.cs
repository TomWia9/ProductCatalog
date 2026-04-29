using Domain.Entities;
using FluentAssertions;
using Infrastructure.Repositories;
using Microsoft.Extensions.Logging.Abstractions;

namespace ProductCatalog.UnitTests;

/// <summary>
/// Unit tests for <see cref="InMemoryProductRepository"/>.
/// </summary>
public sealed class InMemoryProductRepositoryTests
{
    private InMemoryProductRepository CreateRepository()
        => new(NullLogger<InMemoryProductRepository>.Instance);

    [Fact]
    public async Task GetAllAsync_AfterSeeding_ReturnsSeedProducts()
    {
        // Arrange
        var repository = CreateRepository();

        // Act
        var products = await repository.GetAllAsync();

        // Assert - seed data has 3 products
        products.Should().HaveCount(3);
    }

    [Fact]
    public async Task AddAsync_NewProduct_ReturnsAddedProduct()
    {
        // Arrange
        var repository = CreateRepository();
        var product = Product.Create("NEW-001", "New Product", 49.99m);

        // Act
        var result = await repository.AddAsync(product);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(product.Id);
        result.Code.Should().Be("NEW-001");
    }

    [Fact]
    public async Task AddAsync_NewProduct_AppearsInGetAll()
    {
        // Arrange
        var repository = CreateRepository();
        var product = Product.Create("CODE-XYZ", "Test Product", 9.99m);

        // Act
        await repository.AddAsync(product);
        var all = await repository.GetAllAsync();

        // Assert
        all.Should().Contain(p => p.Code == "CODE-XYZ");
    }

    [Fact]
    public async Task ExistsByCodeAsync_ExistingCode_ReturnsTrue()
    {
        // Arrange
        var repository = CreateRepository();
        // "LAPTOP-001" is seeded by default
        const string existingCode = "LAPTOP-001";

        // Act
        var exists = await repository.ExistsByCodeAsync(existingCode);

        // Assert
        exists.Should().BeTrue();
    }

    [Fact]
    public async Task ExistsByCodeAsync_NonExistingCode_ReturnsFalse()
    {
        // Arrange
        var repository = CreateRepository();

        // Act
        var exists = await repository.ExistsByCodeAsync("DOES-NOT-EXIST");

        // Assert
        exists.Should().BeFalse();
    }

    [Fact]
    public async Task ExistsByCodeAsync_IsCaseInsensitive()
    {
        // Arrange
        var repository = CreateRepository();

        // Act
        var existsLower = await repository.ExistsByCodeAsync("laptop-001");
        var existsUpper = await repository.ExistsByCodeAsync("LAPTOP-001");

        // Assert
        existsLower.Should().BeTrue();
        existsUpper.Should().BeTrue();
    }

    [Fact]
    public async Task GetAllAsync_ReturnsProductsOrderedByCreatedAtDescending()
    {
        // Arrange
        var repository = CreateRepository();

        // Add a new product, it should appear first
        await Task.Delay(10); // ensure distinct timestamp
        var newestProduct = Product.Create("NEWEST-001", "Newest Product", 1.00m);
        await repository.AddAsync(newestProduct);

        // Act
        var products = await repository.GetAllAsync();

        // Assert
        products.First().Id.Should().Be(newestProduct.Id);
    }

    [Fact]
    public async Task AddAsync_MultipleProducts_AllAreRetrievable()
    {
        // Arrange
        var repository = CreateRepository();
        var seedCount = (await repository.GetAllAsync()).Count;

        var product1 = Product.Create("MULTI-001", "Product One", 10.00m);
        var product2 = Product.Create("MULTI-002", "Product Two", 20.00m);

        // Act
        await repository.AddAsync(product1);
        await repository.AddAsync(product2);
        var all = await repository.GetAllAsync();

        // Assert
        all.Should().HaveCount(seedCount + 2);
    }
}