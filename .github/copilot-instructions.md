<!-- Copilot instructions for working with the Smart Attendance GPS codebase -->
# Smart Attendance GPS — Copilot Guidance

Purpose: Give AI coding agents the minimal, high-value context to be immediately productive in this repository.

- Project type: ASP.NET Core Web API (Identity + EF Core). The main app project lives in `Smart Attendance GPS/` and is an ASP.NET Core app wired up in `Program.cs`.

- Big picture:
  - `Program.cs`: application bootstrap — DI, JWT authentication, Swagger, CORS, and service registrations (look here first).
  - Controllers: all HTTP endpoints live in `Smart Attendance GPS/Controllers/` and should remain thin — controllers delegate to services.
  - Services: business logic lives in `Smart Attendance GPS/Services/` (interfaces in `Services/IService`). Typical flow: Controller -> Service -> `ApplicationDbContext`.
  - Data layer: EF Core `ApplicationDbContext` at `Models/Context/ApplicationDbContext.cs`. Models are in `Models/` and DTOs in `DTOs/`.
  - Auth: Identity + JWT. `Helpers/JWT.cs` holds config schema; `Services/AuthService.cs` generates tokens. `Program.cs` configures JWT validation and Swagger security.

- Important files to reference when changing behavior:
  - `Program.cs` — DI and middleware ordering (authentication before authorization, Swagger at root).
  - `Models/Context/ApplicationDbContext.cs` — DB set names, precision for coordinates, and Identity table renames.
  - `Seeders/DbSeeder.cs` — seeding roles/users; seeder is registered but commented-out call exists in `Program.cs`.
  - `Helpers/JWT.cs` and `appsettings.json` / `appsettings.Development.json` — JWT values (`Key`, `Issuer`, `Audience`, `DurationInDays`).

- Conventions and patterns (repository-specific):
  - DI-first: services are registered in `Program.cs` with `AddScoped<Interface, Implementation>()`. Prefer adding new services here.
  - Controllers are thin; put reuseable logic into services under `Services/` and interfaces under `Services/IService`.
  - Use DTOs (`DTOs/`) for all incoming/outgoing payloads. Examples: `CreateUserDto.cs`, `UserDto.cs`, `AssignUserToCompanyDto.cs`.
  - Many-to-many user-company mapping is represented by `UserCompany` model and configured as a composite key in `ApplicationDbContext`.
  - Decimal coordinates use `decimal(9,6)` precision for latitude/longitude (configured in `OnModelCreating`).
  - Identity tables are renamed via `OnModelCreating` — queries expecting default Identity table names must respect these custom names.

- Build / run / DB workflows (practical commands):
  - Build solution:
    - `dotnet build "Smart Attendance GPS.sln"`
  - Run the API (project folder contains csproj):
    - `dotnet run --project "Smart Attendance GPS/Smart Attendance GPS.csproj"`
  - Apply EF Core migrations (ensure `dotnet-ef` is available):
    - `dotnet tool install --global dotnet-ef` (once)
    - `dotnet ef database update --project "Smart Attendance GPS/Smart Attendance GPS.csproj" --startup-project "Smart Attendance GPS/Smart Attendance GPS.csproj"`
  - Seed DB manually (uses `DbSeeder.SeedAsync`): create a small runner or uncomment the seed scope in `Program.cs` and run locally.

- Tests: this repo contains no test project. Avoid adding test-only changes without confirming where to place them (ask first).

- Integration points and external dependencies:
  - SQL Server via connection string keyed as `DefaultConnection` in `appsettings.json`.
  - ASP.NET Core Identity (configured in `Program.cs`) — user management uses `UserManager<ApplicationUser>` and `RoleManager<IdentityRole>`.
  - JWT security: modifying token generation or validation requires changing `Helpers/JWT.cs`, `AuthService`, and `Program.cs` together.

- Code examples to follow:
  - Add a new service: create `Services/YourService.cs` + `Services/IService/IYourService.cs`, register in `Program.cs` with `AddScoped<IYourService, YourService>()`, and inject into controllers.
  - Add a DB migration: `dotnet ef migrations add DescriptiveName --project "Smart Attendance GPS/Smart Attendance GPS.csproj"` then `dotnet ef database update`.

- Common pitfalls to avoid:
  - Do not assume default Identity table names — `ApplicationDbContext` renames Identity tables to `Users`, `Roles`, etc.
  - Coordinate columns use explicit precision — changing model types may require updating migration SQL types.
  - `Seeders/DbSeeder` is registered but the automatic invocation in `Program.cs` is commented out; running it requires creating a scope and invoking `SeedAsync`.

If any of these sections are unclear or you'd like examples added (e.g., a small local seeding script or a sample migration command wrapper), tell me which piece to expand and I will iterate.
