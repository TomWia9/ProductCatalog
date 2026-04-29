using Api.Endpoints;
using Application;
using Infrastructure;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

//Logging 
builder.Logging.ClearProviders();
builder.Logging.AddConsole();

//OpenAPI, Scalar
builder.Services.AddOpenApi(options =>
{
    options.AddDocumentTransformer((document, _, _) =>
    {
        document.Info.Title = "Product Catalog API";
        document.Info.Version = "v1";
        document.Info.Description =
            "REST API for managing the product catalog. Built with .NET 10, Minimal API, Clean Architecture and CQRS.";
        return Task.CompletedTask;
    });
});

//CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularApp", policy =>
    {
        policy
            .WithOrigins(
                "http://localhost:4200", //Angular dev server
                "http://localhost:80", //Docker
                "http://frontend:80" //Docker Compose internal
            )
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

//Services
builder.Services.AddApplication();
builder.Services.AddInfrastructure();
builder.Services.AddProblemDetails();

var app = builder.Build();

app.UseExceptionHandler();
app.UseCors("AllowAngularApp");

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(options =>
    {
        options.Title = "Product Catalog API";
        options.Theme = ScalarTheme.DeepSpace;
    });
}

app.MapProductsEndpoints();

app.Run();