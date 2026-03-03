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
2. Run API:
   ```bash
   dotnet run --project src/AgoraCommerce.Api
   ```
3. Open Swagger in development:
   `https://localhost:<port>/swagger`
4. Health endpoint:
   `GET https://localhost:<port>/health`

## Phase Instructions

Required phase instruction files are in the `/docs` folder.
