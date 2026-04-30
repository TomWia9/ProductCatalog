# Product Catalog

A full-stack product catalog application built with **.NET 10 Minimal API** and **Angular 21**, following **Clean Architecture** and **CQRS** principles.

---

## Tech Stack

| Layer              | Technology                           |
| ------------------ | ------------------------------------ |
| Backend Framework  | .NET 10 — Minimal API                |
| Architecture       | Clean Architecture + CQRS (MediatR)  |
| Validation         | FluentValidation                     |
| API Documentation  | Scalar (OpenAPI 3)                   |
| Frontend Framework | Angular 21                           |
| Frontend Forms     | Angular Reactive Forms               |
| Containerization   | Docker + Docker Compose              |
| Testing            | xUnit, FluentAssertions, NSubstitute |
| Data store         | In-memory (ConcurrentDictionary)     |

---

## Project Structure

```
product-catalog/
├── backend/
│   ├── src/
│   │   ├── Api/          # Minimal API endpoints, DI setup
│   │   ├── Application/  # CQRS Commands, Queries, DTOs, Interfaces
│   │   ├── Domain/       # Domain Entities (Product)
│   │   └── Infrastructure/ # In-memory repository
│   ├── tests/
│   │   └── ProductCatalog.UnitTests/        # xUnit unit tests
│   ├── Dockerfile
│   └── ProductCatalog.sln
├── frontend/
│   ├── src/
│   │   └── app/
│   │       ├── core/                    # Models, Services
│   │       └── features/               # ProductForm, ProductList components
│   ├── nginx.conf
│   ├── Dockerfile
│   └── angular.json
└── docker-compose.yml
```

---

## Running with Docker Compose

```bash
# From the project root
docker compose up
```

| URL                                | Description           |
| ---------------------------------- | --------------------- |
| http://localhost                   | Angular frontend      |
| http://localhost:5000/api/products | Products API endpoint |

---

## Running Locally (Development)

### Backend

```bash
cd backend
dotnet restore
dotnet run --project src/Api
```

API runs on `http://localhost:5237`. Scalar UI available at `http://localhost:5237/scalar/v1`.

### Frontend

```bash
cd frontend
npm install
npm start
```

App runs on `http://localhost:4200`.

---

## Running Tests

```bash
cd backend
dotnet test
```

Test coverage includes:

- **Domain**: `Product` entity creation and validation rules
- **Application**: `GetProductsQueryHandler`, `CreateProductCommandHandler`, `CreateProductCommandValidator`
- **Infrastructure**: `InMemoryProductRepository` — all CRUD operations

---

## API Endpoints

### `GET /api/products`

Returns all products ordered by creation date (descending).

**Response `200 OK`:**

```json
[
  {
    "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "code": "LAPTOP-001",
    "name": "Dell XPS 15 Laptop",
    "price": 5999.99,
    "createdAtUtc": "2025-01-01T12:00:00Z"
  }
]
```

### `POST /api/products`

Adds a new product to the catalog.

**Request body:**

```json
{
  "code": "MOUSE-NEW",
  "name": "Logitech G Pro X",
  "price": 299.99
}
```

**Responses:**

- `201 Created` — product created successfully
- `400 Bad Request` — validation errors (empty fields, non-positive price)
- `409 Conflict` — product with this code already exists
