# LeaveFlowHR

A backend REST API for managing employee leave requests with a sequential approval flow (Manager → HR).

## Features

- JWT-based authentication
- Leave request submission with business-day calculation
- Sequential approval flow: **Manager → HR** (or direct to HR if no manager assigned)
- Leave balance tracking per year (entitled, used, pending days)
- Role-based access: **Employee**, **Manager**, **HR**, **Admin**
- Database seeding with default admin user

## Tech Stack

- **ASP.NET Core** — Web API framework
- **Entity Framework Core** — ORM and database migrations
- **SQL Server** — Database
- **FluentValidation** — Request validation

---

## Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/download)
- [SQL Server](https://www.microsoft.com/en-us/sql-server/sql-server-downloads) (or SQL Server Express)
- [Docker](https://www.docker.com/) _(optional)_
- [Make](https://www.gnu.org/software/make/) _(optional, for Makefile commands)_

---

## Getting Started

### 1. Clone the repository

```bash
git clone https://github.com/alfianhs/leaveflowhr-service-api.git
cd leaveflowhr-service-api
```

### 2. Configure environment

Copy the example config and fill in your values:

```bash
cp src/Api/appsettings.example.json src/Api/appsettings.json
```

Edit `src/Api/appsettings.json`:

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*",
  "Environment": "Development",
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost, 1433;Initial Catalog=your_db_name;User ID=sa;Password=your_db_password;TrustServerCertificate=True;Connection Timeout=30;"
  },
  "Jwt": {
    "Issuer": "LeaveFlowHR",
    "Audience": "LeaveFlowHR",
    "SecretKey": "secret-128-bit-key-here",
    "ExpiryInMinutes": 60
  }
}
```

### 3. Run database migrations

```bash
make migrate-up
```

Or without Make:

```bash
dotnet ef database update --context AppDbContext --project src/Api/Api.csproj
```

### 4. Run the API

```bash
make watch-api
```

Or without Make:

```bash
dotnet watch --project src/Api/Api.csproj
```

The API will be available at `http://localhost:5218`.
 
Swagger documentation is accessible at `http://localhost:5218/swagger/index.html` when `Environment` is set to `Development`.

---

## Seeded Data
 
The database is automatically seeded on first run via `Infrastructure/Database/DbSeeder.cs`. Default accounts:
 
| Name | Email | Password | Role |
|---|---|---|---|
| Admin | admin@mail.com | `securepassword` | Admin |
| HR Admin | hr@mail.com | `securepassword` | HR |
| Manager | manager@mail.com | `securepassword` | Manager |
| Employee | employee@mail.com | `securepassword` | Employee |

Leave balances for the current year are automatically generated for all users except those with the Admin role.
 
> These accounts are for development purposes only. Replace or remove seeded credentials before deploying to production.

## Available Make Commands

| Command | Description |
|---|---|
| `make build-api` | Build the API project |
| `make watch-api` | Run the API in watch mode |
| `make migration name=MigrationName` | Add a new migration |
| `make migration-remove` | Remove the latest migration |
| `make migrate-up` | Apply all pending migrations |
| `make migrate-down target=MigrationName` | Rollback to a specific migration |

---

## Running with Docker

### 1. Build the image

```bash
docker build -t leaveflowhr .
```

### 2. Run the container

When your app runs inside Docker, `localhost` refers to the container itself — not your host machine. So you need a special address to reach SQL Server running on your PC.

**Windows / Mac** — use `host.docker.internal`:

```bash
docker run -p 8080:8080 --name leaveflowhr -d \
  -e ConnectionStrings__DefaultConnection="Server=host.docker.internal, 1433;Initial Catalog=your_db_name;User ID=sa;Password=your_db_password;TrustServerCertificate=True;Connection Timeout=30;" \
  -e Jwt__Issuer="LeaveFlowHR" \
  -e Jwt__Audience="LeaveFlowHRUsers" \
  -e Jwt__SecretKey="secret-128-bit-key-here" \
  -e Jwt__ExpiryInMinutes=60 \
  leaveflowhr
```

**Linux** — `host.docker.internal` is not available by default, use `--network host` and `localhost` instead:

```bash
docker run --network host --name leaveflowhr -d \
  -e ConnectionStrings__DefaultConnection="Server=host.docker.internal, 1433;Initial Catalog=your_db_name;User ID=sa;Password=your_db_password;TrustServerCertificate=True;Connection Timeout=30;" \
  -e Jwt__Issuer="LeaveFlowHR" \
  -e Jwt__Audience="LeaveFlowHRUsers" \
  -e Jwt__SecretKey="secret-128-bit-key-here" \
  -e Jwt__ExpiryInMinutes=60 \
  leaveflowhr
```

**Or you can use the .env file to setup docker container with simplified command line.**

Copy the .env.example and fill in your values:

```bash
cp src/Api/.env.example src/Api/.env
```

Edit `src/Api/.env`:

```properties
Logging__LogLevel__Default=Information
Logging__LogLevel__MicrosoftAspNetCore=Warning
AllowedHosts=*
Environment=Development/Production

# DB Connection String
ConnectionStrings__DefaultConnection=Server=host.docker.internal, 1433;Initial Catalog=your_db_name;User ID=sa;Password=your_db_password;TrustServerCertificate=True;Connection Timeout=30;

# JWT Settings
Jwt__Issuer=LeaveFlowHR
Jwt__Audience=LeaveFlowHRUsers
Jwt__SecretKey=secret-128-bit-key-here
Jwt__ExpiryInMinutes=60
```

Run command:
```bash
docker run -p 8080:8080 --name leaveflowhr -d --env-file src/Api/.env leaveflowhr
```

The API will be available at `http://localhost:8080`.

---

## Project Structure

```
.
├── src
│   └── Api
│       ├── Common          # Shared entities, responses, results, utilities
│       ├── Configurations  # App configuration bindings
│       ├── Extensions      # App service extensions
│       ├── Infrastructure  # Database, migrations, third-party services
│       ├── Modules         # App modules with Controller and Service pattern
│       │   ├── Auth
│       │   ├── LeaveRequests
│       │   └── Users
│       ├── Properties      # Launch settings
│       └── Program.cs
├── Dockerfile
├── Makefile
└── LeaveFlowHR.sln
```
