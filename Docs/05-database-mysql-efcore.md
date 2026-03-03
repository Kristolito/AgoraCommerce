# Database Setup

## Packages
- Microsoft.EntityFrameworkCore
- Pomelo.EntityFrameworkCore.MySql

## Migration Commands
dotnet ef migrations add Initial
dotnet ef database update

## Important Indexes
- Product.Sku unique
- Coupon.Code unique
- Order.UserId index
