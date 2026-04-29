using Application.DTOs;
using Application.Interfaces;
using Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Commands;

/// <summary>
/// Handler for <see cref="CreateProductCommand"/>.
/// Validates, creates, and persists a new product.
/// </summary>
public sealed class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, ProductDto>
{
    private readonly IProductRepository _productRepository;
    private readonly ILogger<CreateProductCommandHandler> _logger;

    /// <summary>
    /// Initializes a new instance of <see cref="CreateProductCommandHandler"/>.
    /// </summary>
    /// <param name="productRepository">The product repository.</param>
    /// <param name="logger">The logger instance.</param>
    public CreateProductCommandHandler(
        IProductRepository productRepository,
        ILogger<CreateProductCommandHandler> logger)
    {
        _productRepository = productRepository;
        _logger = logger;
    }

    /// <summary>
    /// Handles the <see cref="CreateProductCommand"/> request.
    /// </summary>
    /// <param name="request">The command request.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>The newly created <see cref="ProductDto"/>.</returns>
    /// <exception cref="InvalidOperationException">Thrown when a product with the same code already exists.</exception>
    public async Task<ProductDto> Handle(
        CreateProductCommand request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Creating product with code '{Code}'.", request.Code);

        var codeExists = await _productRepository.ExistsByCodeAsync(request.Code, cancellationToken);
        if (codeExists)
        {
            _logger.LogWarning("Product with code '{Code}' already exists.", request.Code);
            throw new InvalidOperationException($"A product with code '{request.Code}' already exists.");
        }

        var product = Product.Create(request.Code, request.Name, request.Price);
        var savedProduct = await _productRepository.AddAsync(product, cancellationToken);

        _logger.LogInformation("Product '{Code}' created with Id {Id}.", savedProduct.Code, savedProduct.Id);

        return new ProductDto(savedProduct.Id, savedProduct.Code, savedProduct.Name, savedProduct.Price, savedProduct.CreatedAtUtc);
    }
}