using Application.DTOs;
using MediatR;

namespace Application.Commands;

/// <summary>
/// Command object for adding a new product to the catalog.
/// Implements the CQRS Command pattern via MediatR.
/// </summary>
/// <param name="Code">The product code (SKU).</param>
/// <param name="Name">The product name.</param>
/// <param name="Price">The product price.</param>
public sealed record CreateProductCommand(
    string Code,
    string Name,
    decimal Price
) : IRequest<ProductDto>;