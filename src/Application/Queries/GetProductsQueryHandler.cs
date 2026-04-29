using Application.DTOs;
using Application.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Queries;

/// <summary>
/// Handler for <see cref="GetProductsQuery"/>.
/// Retrieves all products and maps them to DTOs.
/// </summary>
public sealed class GetProductsQueryHandler
    : IRequestHandler<GetProductsQuery, IReadOnlyCollection<ProductDto>>
{
    private readonly IProductRepository _productRepository;
    private readonly ILogger<GetProductsQueryHandler> _logger;

    /// <summary>
    /// Initializes a new instance of <see cref="GetProductsQueryHandler"/>.
    /// </summary>
    /// <param name="productRepository">The product repository.</param>
    /// <param name="logger">The logger instance.</param>
    public GetProductsQueryHandler(
        IProductRepository productRepository,
        ILogger<GetProductsQueryHandler> logger)
    {
        _productRepository = productRepository;
        _logger = logger;
    }

    /// <summary>
    /// Handles the <see cref="GetProductsQuery"/> request.
    /// </summary>
    /// <param name="request">The query request.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>A read-only collection of <see cref="ProductDto"/>.</returns>
    public async Task<IReadOnlyCollection<ProductDto>> Handle(
        GetProductsQuery request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Retrieving all products from the catalog.");

        var products = await _productRepository.GetAllAsync(cancellationToken);

        var dtos = products
            .Select(p => new ProductDto(p.Id, p.Code, p.Name, p.Price, p.CreatedAtUtc))
            .ToList()
            .AsReadOnly();

        _logger.LogInformation("Retrieved {Count} products.", dtos.Count);

        return dtos;
    }
}