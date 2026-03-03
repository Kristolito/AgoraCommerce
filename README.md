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
   dotnet ef migrations add InitialCreate --project src/AgoraCommerce.Infrastructure --startup-project src/AgoraCommerce.Api
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

## Phase Instructions

Required phase instruction files are in the `/docs` folder.
