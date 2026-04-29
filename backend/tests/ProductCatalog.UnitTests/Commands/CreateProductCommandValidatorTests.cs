using Application.Commands;
using FluentAssertions;

namespace ProductCatalog.UnitTests.Commands;

/// <summary>
/// Unit tests for <see cref="CreateProductCommandValidator"/>.
/// </summary>
public sealed class CreateProductCommandValidatorTests
{
    private readonly CreateProductCommandValidator _validator = new();

    [Theory]
    [InlineData("CODE-001", "Valid Product", 1.00)]
    [InlineData("SKU", "Name", 0.01)]
    [InlineData("LONG-CODE-VALID", "Product With Long Name", 9999.99)]
    public async Task Validate_ValidCommand_PassesValidation(string code, string name, decimal price)
    {
        // Arrange
        var command = new CreateProductCommand(code, name, price);

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Theory]
    [InlineData("", "Product", 10.00, "Code")]
    [InlineData("CODE", "", 10.00, "Name")]
    public async Task Validate_EmptyRequiredField_FailsValidation(
        string code, string name, decimal price, string expectedPropertyName)
    {
        // Arrange
        var command = new CreateProductCommand(code, name, price);

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == expectedPropertyName);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-999.99)]
    public async Task Validate_NonPositivePrice_FailsValidation(decimal price)
    {
        // Arrange
        var command = new CreateProductCommand("CODE", "Product", price);

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Price");
    }

    [Fact]
    public async Task Validate_CodeExceedsMaxLength_FailsValidation()
    {
        // Arrange
        var tooLongCode = new string('X', 51);
        var command = new CreateProductCommand(tooLongCode, "Product", 10.00m);

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Code");
    }
}