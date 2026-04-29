using Application.Commands;
using Application.DTOs;
using Application.Queries;
using FluentValidation;
using MediatR;

namespace Api.Endpoints;

/// <summary>
/// Defines and registers all HTTP endpoints related to the Products resource.
/// Follows the Minimal API endpoint grouping pattern to keep route definitions
/// co-located and separate from the application entry point.
/// </summary>
public static class ProductsEndpoints
{
    /// <summary>
    /// Maps all Products-related endpoints onto the provided <see cref="IEndpointRouteBuilder"/>.
    /// </summary>
    /// <param name="app">The <see cref="IEndpointRouteBuilder"/> to register routes on.</param>
    /// <returns>The same <see cref="IEndpointRouteBuilder"/> instance for fluent chaining.</returns>
    public static IEndpointRouteBuilder MapProductsEndpoints(this IEndpointRouteBuilder app)
    {
        var productsGroup = app.MapGroup("/api/products")
            .WithTags("Products");

        productsGroup.MapGet("/", async (IMediator mediator, CancellationToken ct) =>
            {
                var products = await mediator.Send(new GetProductsQuery(), ct);
                return Results.Ok(products);
            })
            .WithName("GetProducts")
            .WithSummary("Get all products")
            .WithDescription("Returns a list of all products in the catalog, ordered by creation date descending.")
            .Produces<IReadOnlyCollection<ProductDto>>(StatusCodes.Status200OK);

        productsGroup.MapPost("/", async (
                CreateProductCommand command,
                IMediator mediator,
                IValidator<CreateProductCommand> validator,
                CancellationToken ct) =>
            {
                var validationResult = await validator.ValidateAsync(command, ct);
                if (!validationResult.IsValid)
                {
                    var errors = validationResult.Errors
                        .GroupBy(e => e.PropertyName)
                        .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());

                    return Results.ValidationProblem(errors);
                }

                try
                {
                    var product = await mediator.Send(command, ct);
                    return Results.Created($"/api/products/{product.Id}", product);
                }
                catch (InvalidOperationException ex)
                {
                    return Results.Conflict(new { error = ex.Message });
                }
            })
            .WithName("CreateProduct")
            .WithSummary("Create a new product")
            .WithDescription("Adds a new product to the catalog. The product code must be unique.")
            .Produces<ProductDto>(StatusCodes.Status201Created)
            .ProducesValidationProblem()
            .Produces(StatusCodes.Status409Conflict);

        app.MapGet("/health", () => Results.Ok(new { status = "healthy", timestamp = DateTime.UtcNow }))
            .WithTags("Health")
            .WithName("HealthCheck")
            .ExcludeFromDescription();

        return app;
    }
}