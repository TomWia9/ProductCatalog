using Domain.Entities;

namespace Application.Interfaces;

/// <summary>
/// Defines the contract for the product repository.
/// Follows the Repository pattern to abstract data access from the application layer.
/// </summary>
public interface IProductRepository
{
    /// <summary>
    /// Retrieves all products from the catalog asynchronously.
    /// </summary>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>A read-only collection of all <see cref="Product"/> entities.</returns>
    Task<IReadOnlyCollection<Product>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds a new product to the catalog asynchronously.
    /// </summary>
    /// <param name="product">The <see cref="Product"/> entity to add.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>The added <see cref="Product"/> entity.</returns>
    Task<Product> AddAsync(Product product, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks whether a product with the specified code already exists.
    /// </summary>
    /// <param name="code">The product code to check.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns><c>true</c> if a product with the given code exists; otherwise, <c>false</c>.</returns>
    Task<bool> ExistsByCodeAsync(string code, CancellationToken cancellationToken = default);
}
