using Domain.Entities;
using FluentAssertions;

namespace ProductCatalog.UnitTests;

/// <summary>
/// Unit tests for the <see cref="Product"/> domain entity.
/// </summary>
public sealed class ProductEntityTests
{
    [Fact]
    public void Create_ValidParameters_ReturnsProductWithCorrectValues()
    {
        // Act
        var product = Product.Create("TEST-001", "Test Product", 99.99m);

        // Assert
        product.Id.Should().NotBeEmpty();
        product.Code.Should().Be("TEST-001");
        product.Name.Should().Be("Test Product");
        product.Price.Should().Be(99.99m);
        product.CreatedAtUtc.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_InvalidCode_ThrowsArgumentException(string? invalidCode)
    {
        // Act
        var act = () => Product.Create(invalidCode!, "Name", 10.00m);

        // Assert
        act.Should().Throw<ArgumentException>().WithParameterName("code");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_InvalidName_ThrowsArgumentException(string? invalidName)
    {
        // Act
        var act = () => Product.Create("CODE", invalidName!, 10.00m);

        // Assert
        act.Should().Throw<ArgumentException>().WithParameterName("name");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-0.01)]
    public void Create_NonPositivePrice_ThrowsArgumentException(decimal invalidPrice)
    {
        // Act
        var act = () => Product.Create("CODE", "Name", invalidPrice);

        // Assert
        act.Should().Throw<ArgumentException>().WithParameterName("price");
    }

    [Fact]
    public void Create_CodeWithWhitespace_TrimsCode()
    {
        // Act
        var product = Product.Create("  CODE-001  ", "Name", 10.00m);

        // Assert
        product.Code.Should().Be("CODE-001");
    }

    [Fact]
    public void Create_EachProductGetsUniqueId()
    {
        // Act
        var product1 = Product.Create("CODE-A", "Product A", 10.00m);
        var product2 = Product.Create("CODE-B", "Product B", 20.00m);

        // Assert
        product1.Id.Should().NotBe(product2.Id);
    }
}