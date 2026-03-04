# AgoraCommerce

AgoraCommerce is a .NET 8 Clean Architecture solution scaffold for an e-commerce platform.

## Architecture

```text
                 +----------------------+
                 |  AgoraCommerce.Api   |
                 +----------+-----------+
                            |
      +---------------------+----------------------+
      |                                            |
+-----v-------------------+           +------------v-------------+
| AgoraCommerce.Application|          | AgoraCommerce.Contracts  |
+-----------+--------------+          +--------------------------+
            |
+-----------v--------------+
|   AgoraCommerce.Domain   |
+--------------------------+

+--------------------------+
| AgoraCommerce.Infrastructure |
+--------------------------+
References: Application + Domain
```

## Run the API

1. Build solution:
   ```bash
   dotnet build AgoraCommerce.sln
   ```
2. Start MySQL with Docker:
   ```bash
   docker compose up -d
   ```
3. Apply EF Core migrations:
   ```bash
   dotnet ef database update --project src/AgoraCommerce.Infrastructure --startup-project src/AgoraCommerce.Api
   ```
4. Run API:
   ```bash
   dotnet run --project src/AgoraCommerce.Api
   ```
5. Open Swagger in development:
   `https://localhost:<port>/swagger`
6. Health endpoint:
   `GET https://localhost:<port>/health`

## Catalog Examples

Create category (admin):
```bash
curl -X POST http://localhost:5085/api/v1/admin/categories \
  -H "Content-Type: application/json" \
  -d '{"name":"Electronics","slug":"electronics"}'
```

Create product (admin):
```bash
curl -X POST http://localhost:5085/api/v1/admin/products \
  -H "Content-Type: application/json" \
  -d '{"sku":"SKU-1001","name":"Laptop","description":"14-inch laptop","price":999.99,"currency":"GBP","categoryId":"<CATEGORY_ID>","brand":"AgoraBrand"}'
```

Public products list with filters/sort:
```bash
curl "http://localhost:5085/api/v1/products?page=1&pageSize=20&search=laptop&sort=price_desc"
```

Soft delete product (admin):
```bash
curl -X DELETE http://localhost:5085/api/v1/admin/products/<PRODUCT_ID>
```

## Basket Examples

Basket uses `X-Anonymous-Id` for anonymous carts. If the header is missing, the API generates one and returns it in response headers.

Get basket:
```bash
curl -H "X-Anonymous-Id: <ANON_GUID>" \
  http://localhost:5085/api/v1/basket
```

Add basket item:
```bash
curl -X POST http://localhost:5085/api/v1/basket/items \
  -H "Content-Type: application/json" \
  -H "X-Anonymous-Id: <ANON_GUID>" \
  -d '{"productId":"<PRODUCT_ID>","quantity":2}'
```

Update basket item quantity:
```bash
curl -X PUT http://localhost:5085/api/v1/basket/items/<PRODUCT_ID> \
  -H "Content-Type: application/json" \
  -H "X-Anonymous-Id: <ANON_GUID>" \
  -d '{"quantity":3}'
```

Remove basket item:
```bash
curl -X DELETE http://localhost:5085/api/v1/basket/items/<PRODUCT_ID> \
  -H "X-Anonymous-Id: <ANON_GUID>"
```

Clear basket:
```bash
curl -X DELETE http://localhost:5085/api/v1/basket \
  -H "X-Anonymous-Id: <ANON_GUID>"
```

## Tests

Unit tests:
```bash
dotnet test tests/AgoraCommerce.UnitTests
```

Integration tests (expects MySQL from `docker compose up -d`):
```bash
dotnet test tests/AgoraCommerce.IntegrationTests
```

## Phase Instructions

Required phase instruction files are in the `/docs` folder.
