using System.Collections.Concurrent;
using Application.Interfaces;
using Domain.Entities;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Repositories;

/// <summary>
/// Thread-safe in-memory implementation of <see cref="IProductRepository"/>.
/// Uses a <see cref="ConcurrentDictionary{TKey, TValue}"/> as the backing store.
/// Registered as a singleton to persist data for the lifetime of the application.
/// </summary>
public sealed class InMemoryProductRepository : IProductRepository
{
    private readonly ConcurrentDictionary<Guid, Product> _store = new();
    private readonly ILogger<InMemoryProductRepository> _logger;

    /// <summary>
    /// Initializes a new instance of <see cref="InMemoryProductRepository"/>.
    /// Seeds the store with sample products for demonstration purposes.
    /// </summary>
    /// <param name="logger">The logger instance.</param>
    public InMemoryProductRepository(ILogger<InMemoryProductRepository> logger)
    {
        _logger = logger;
        SeedData();
    }

    /// <inheritdoc />
    public Task<IReadOnlyCollection<Product>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Fetching all products from in-memory store. Count: {Count}", _store.Count);

        IReadOnlyCollection<Product> result = _store.Values
            .OrderByDescending(p => p.CreatedAtUtc)
            .ToList()
            .AsReadOnly();

        return Task.FromResult(result);
    }

    /// <inheritdoc />
    public Task<Product> AddAsync(Product product, CancellationToken cancellationToken = default)
    {
        _store[product.Id] = product;
        _logger.LogDebug("Product {Id} added to in-memory store.", product.Id);
        return Task.FromResult(product);
    }

    /// <inheritdoc />
    public Task<bool> ExistsByCodeAsync(string code, CancellationToken cancellationToken = default)
    {
        var exists = _store.Values.Any(p =>
            string.Equals(p.Code, code, StringComparison.OrdinalIgnoreCase));
        return Task.FromResult(exists);
    }

    /// <summary>
    /// Seeds the in-memory store with initial sample products.
    /// </summary>
    private void SeedData()
    {
        var sampleProducts = new[]
        {
            Product.Create("LAPTOP-001", "Dell XPS 15 Laptop", 5999.99m),
            Product.Create("MOUSE-002", "Logitech MX Master 3 Mouse", 429.00m),
            Product.Create("KEYBOARD-003", "Keychron K2 Mechanical Keyboard", 349.00m),
        };

        foreach (var product in sampleProducts)
        {
            _store[product.Id] = product;
        }

        _logger.LogInformation("In-memory store seeded with {Count} sample products.", sampleProducts.Length);
    }
}
