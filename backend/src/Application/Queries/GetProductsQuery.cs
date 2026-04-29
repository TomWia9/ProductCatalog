using Application.DTOs;
using MediatR;

namespace Application.Queries;

/// <summary>
/// Query object for retrieving all products from the catalog.
/// Implements the CQRS Query pattern via MediatR.
/// </summary>
public sealed record GetProductsQuery : IRequest<IReadOnlyCollection<ProductDto>>;


