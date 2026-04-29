namespace Domain.Entities;

/// <summary>
/// Represents a product in the catalog.
/// </summary>
public sealed class Product
{
    /// <summary>
    /// Gets the unique identifier of the product.
    /// </summary>
    public Guid Id { get; private set; }

    /// <summary>
    /// Gets the product code (SKU).
    /// </summary>
    public string Code { get; private set; } = string.Empty;

    /// <summary>
    /// Gets the name of the product.
    /// </summary>
    public string Name { get; private set; } = string.Empty;

    /// <summary>
    /// Gets the price of the product in PLN.
    /// </summary>
    public decimal Price { get; private set; }

    /// <summary>
    /// Gets the UTC timestamp when the product was created.
    /// </summary>
    public DateTime CreatedAtUtc { get; private set; }

    // Private constructor for EF / serialization
    private Product()
    {
    }

    /// <summary>
    /// Creates a new product instance with the specified properties.
    /// </summary>
    /// <param name="code">The product code (SKU). Must not be null or whitespace.</param>
    /// <param name="name">The product name. Must not be null or whitespace.</param>
    /// <param name="price">The product price. Must be greater than zero.</param>
    /// <returns>A new <see cref="Product"/> instance.</returns>
    /// <exception cref="ArgumentException">Thrown when code or name is null/empty, or price is not positive.</exception>
    public static Product Create(string code, string name, decimal price)
    {
        if (string.IsNullOrWhiteSpace(code))
            throw new ArgumentException("Product code cannot be empty.", nameof(code));

        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Product name cannot be empty.", nameof(name));

        if (price <= 0)
            throw new ArgumentException("Product price must be greater than zero.", nameof(price));

        return new Product
        {
            Id = Guid.NewGuid(),
            Code = code.Trim(),
            Name = name.Trim(),
            Price = price,
            CreatedAtUtc = DateTime.UtcNow
        };
    }
}