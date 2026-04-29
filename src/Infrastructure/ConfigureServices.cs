using Application.Interfaces;
using Infrastructure.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure;

/// <summary>
/// Provides extension methods for registering Infrastructure layer services
/// into the dependency injection container.
/// </summary>
public static class ConfigureServices
{
    /// <summary>
    /// Registers all Infrastructure layer dependencies with the DI container.
    /// Adds the in-memory implementation of <see cref="IProductRepository"/>
    /// as a singleton to preserve catalog state for the application lifetime.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add services to.</param>
    /// <returns>The same <see cref="IServiceCollection"/> instance for fluent chaining.</returns>
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddSingleton<IProductRepository, InMemoryProductRepository>();

        return services;
    }
}