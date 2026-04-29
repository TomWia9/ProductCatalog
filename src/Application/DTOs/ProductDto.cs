namespace Application.DTOs;

/// <summary>
/// Data Transfer Object representing a product returned from the API.
/// </summary>
/// <param name="Id">The unique identifier of the product.</param>
/// <param name="Code">The product code (SKU).</param>
/// <param name="Name">The name of the product.</param>
/// <param name="Price">The price of the product.</param>
/// <param name="CreatedAtUtc">The UTC timestamp when the product was created.</param>
public sealed record ProductDto(
    Guid Id,
    string Code,
    string Name,
    decimal Price,
    DateTime CreatedAtUtc
);