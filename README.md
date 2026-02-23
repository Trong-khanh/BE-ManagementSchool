# BE-ManagementSchool

Backend API for school management built with ASP.NET Core 6, Entity Framework Core, PostgreSQL, Identity, JWT, and Swagger.

## Prerequisites

- .NET SDK 6.0.x
- PostgreSQL 14+
- Optional: Docker + Docker Compose

## Run locally

1. Configure secrets using environment variables (recommended):

```bash
export ConnectionStrings__DefaultConnection="Host=localhost;Port=5432;Database=managementschool;Username=ms_user;Password=ms_pass"
export JWT__Secret="CHANGE_THIS_TO_A_LONG_RANDOM_SECRET"
export JWT__validIssuer="http://localhost:5226"
export JWT__validAudience="http://localhost:5226"
export EmailConfiguration__Username="SMTP_USERNAME"
export EmailConfiguration__Password="SMTP_PASSWORD"
export MomoAPI__AccessKey="MOMO_ACCESS_KEY"
export MomoAPI__SecretKey="MOMO_SECRET_KEY"
```

2. Start API:

```bash
dotnet run --project ManagementSchool/ManagementSchool.csproj
```

3. Open Swagger:

- `http://localhost:5226/swagger`

## Run with Docker

```bash
docker compose up --build
```

Swagger:

- `http://localhost:8080/swagger`

## Security notes

- Do not commit real secrets to `appsettings*.json`, `docker-compose.yml`, or scripts.
- Configure `Cors:AllowedOrigins` explicitly for each environment.
- Keep JWT issuer/audience/secret in environment variables for production.
