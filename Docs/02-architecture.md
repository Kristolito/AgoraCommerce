# Architecture

## Layers
- Domain (Entities, Value Objects)
- Application (Use Cases)
- Infrastructure (EF Core, MySQL)
- API (Controllers, Middleware)

## Rules
- Domain depends on nothing
- Application depends on Domain
- Infrastructure depends on Application + Domain
- API depends on Application only
