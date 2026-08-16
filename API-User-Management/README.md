# User Management API

ASP.NET Core 8 Web API implementing:

- RESTful CRUD endpoints
- Clean Architecture
- SOLID-oriented separation of concerns
- EF Core + SQLite
- Configurable SQLite database path (`/data/app.db` by default)
- Input validation through DataAnnotations + `[ApiController]`
- Correct HTTP status codes
- Swagger/OpenAPI
- OAuth2/OIDC JWT bearer validation
- Public GET endpoints; POST/PUT/DELETE require `Authorization: Bearer <JWT>`

## Architecture

```text
UserManagement
├── src
│   ├── UserManagement.Api
│   │   └── Controllers
│   ├── UserManagement.Application
│   │   ├── Abstractions
│   │   ├── DTOs
│   │   ├── Exceptions
│   │   └── Services
│   ├── UserManagement.Domain
│   │   └── Entities
│   └── UserManagement.Infrastructure
│       └── Persistence
└── README.md
```

### Responsibilities

- **Domain**: business entity and invariants. No framework dependency.
- **Application**: use cases, DTOs and repository abstractions.
- **Infrastructure**: EF Core, SQLite and repository implementation.
- **API**: HTTP, authentication, Swagger and controllers.

This follows Dependency Inversion: Application depends on `IUserRepository`, while Infrastructure implements it.

## Create/build

Install .NET 8 SDK, then from the solution directory:

```powershell
dotnet restore
dotnet build
dotnet run --project .\src\UserManagement.Api\UserManagement.Api.csproj
```

Swagger is available in Development at:

```text
https://localhost:<port>/swagger
```

## Database

Default:

```text
/data/app.db
```

Override it using configuration:

```powershell
$env:Database__Path="C:\data\app.db"
dotnet run --project .\src\UserManagement.Api\UserManagement.Api.csproj
```

For Windows, `/data/app.db` may resolve to a root-level `data` directory. For local development, setting `Database__Path` to a Windows path is recommended.

The sample uses `EnsureCreatedAsync()` to make the project immediately runnable. For production, use EF Core migrations.

## JWT / OAuth2 / OIDC

Configure:

```json
"Jwt": {
  "Authority": "https://your-identity-provider.example.com",
  "Audience": "user-management-api",
  "RequireHttpsMetadata": true
}
```

The identity provider should expose standard OIDC discovery metadata and signing keys.

Protected operations:

- `POST /api/users`
- `PUT /api/users/{id}`
- `DELETE /api/users/{id}`

Public operations:

- `GET /api/users`
- `GET /api/users/{id}`

This choice means a UI can show the list without login, while any add/edit/delete page/action must require login.

## Endpoints

| Method | Endpoint | Auth | Success |
|---|---|---|---|
| GET | `/api/users` | Public | 200 |
| GET | `/api/users/{id}` | Public | 200 / 404 |
| POST | `/api/users` | JWT | 201 / 400 / 409 |
| PUT | `/api/users/{id}` | JWT | 200 / 400 / 404 / 409 |
| DELETE | `/api/users/{id}` | JWT | 204 / 404 |

## Example JSON

POST:

```json
{
  "name": "John Doe",
  "email": "john.doe@example.com"
}
```

Validation:

- Name required, 2–100 characters
- Email required, valid email format, max 255 characters
- Email is unique

## Design patterns used

1. **Clean Architecture** — isolates domain, application, infrastructure and API.
2. **Repository Pattern** — application depends on `IUserRepository`.
3. **Dependency Injection** — all services/repositories are injected.
4. **DTO Pattern** — API contracts do not expose domain entities.
5. **Service Layer** — business use cases live outside controllers.
6. **Options/configuration pattern** — database/JWT configuration comes from application configuration.

## Production improvements

For a production system, add:

- EF Core migrations instead of `EnsureCreated`
- centralized exception middleware / `ProblemDetails`
- structured logging and OpenTelemetry
- pagination/filtering/sorting for `GET /api/users`
- rate limiting
- refresh-token/session handling at the identity-provider layer
- authorization policies/roles/scopes
- integration tests using a test database
- Docker volume mapped to `/data`
- secrets stored outside `appsettings.json`
