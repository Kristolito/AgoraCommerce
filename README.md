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

## Checkout & Orders Examples

Checkout requires both headers:
- `X-Anonymous-Id` for basket/order ownership (when no auth)
- `Idempotency-Key` to make retries safe

Create order from basket:
```bash
curl -X POST http://localhost:5085/api/v1/checkout \
  -H "Content-Type: application/json" \
  -H "X-Anonymous-Id: <ANON_GUID>" \
  -H "Idempotency-Key: <UNIQUE_KEY>" \
  -d '{
    "shippingAddress": {
      "line1": "221B Baker Street",
      "line2": "",
      "city": "London",
      "postcode": "NW1",
      "country": "GB"
    },
    "couponCode": "SAVE10"
  }'
```

Idempotency behavior:
- First request with a new `Idempotency-Key` creates an order (`201 Created`)
- Repeating the same request with the same key returns the same order (`200 OK`) without creating duplicates

Get orders for owner:
```bash
curl -H "X-Anonymous-Id: <ANON_GUID>" \
  "http://localhost:5085/api/v1/orders?page=1&pageSize=20"
```

Get order by id:
```bash
curl -H "X-Anonymous-Id: <ANON_GUID>" \
  "http://localhost:5085/api/v1/orders/<ORDER_ID>"
```

## Coupons Examples

Create coupon (admin):
```bash
curl -X POST http://localhost:5085/api/v1/admin/coupons \
  -H "Content-Type: application/json" \
  -d '{
    "code":"SAVE10",
    "type":"Percent",
    "amount":10,
    "currency":null,
    "isActive":true,
    "activeFrom":null,
    "activeTo":null,
    "maxRedemptions":100
  }'
```

Validate coupon:
```bash
curl -X POST http://localhost:5085/api/v1/coupons/validate \
  -H "Content-Type: application/json" \
  -d '{"code":"SAVE10","subtotal":100}'
```

List coupons (admin):
```bash
curl "http://localhost:5085/api/v1/admin/coupons?page=1&pageSize=20"
```

Coupon checkout behavior:
- If `couponCode` is valid at checkout, response includes `subtotal`, `discount`, `total`, and `couponCode`
- Coupon redemption is incremented only once for a successful checkout transaction
- Retrying checkout with the same `Idempotency-Key` returns the same order and does not redeem coupon again

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
