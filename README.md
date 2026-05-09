# Smart Attendance GPS

A **ASP.NET Core 8 Web API** for employee attendance management using **GPS location tracking** and **Face Recognition**.

## What it does

This system allows companies to manage employee check-ins and check-outs with location verification and biometric face authentication. It supports multiple companies on a single platform with role-based access control.

## Key Features

- **JWT Authentication** — Secure login for Admins, Managers, and Employees
- **GPS-based Check-in / Check-out** — Employees record attendance with their current location
- **Face Recognition** — Biometric face enrollment and verification before clocking in
- **Company Management** — Multi-company support; each company manages its own employees
- **Role System** — Admin, Manager, and Employee roles with separate permissions
- **Swagger UI** — Full interactive API documentation available out of the box

## Tech Stack

- ASP.NET Core 8 Web API
- Entity Framework Core + SQL Server
- ASP.NET Identity + JWT Bearer
- Face Recognition integration (FaceRecognition.Core)
- Swagger / OpenAPI

## Project Structure

```
Controllers/
  AuthController       → Register, login, password management
  AdminController      → System-wide admin operations
  CompanyController    → Create and manage companies
  EmployeeController   → Employee CRUD and assignment to companies
  FaceController       → Face enrollment and verification

Models/
  ApplicationUser      → Extended Identity user
  Attendance           → Check-in/out records with GPS coordinates
  Company              → Company entity
  UserCompany          → Many-to-many: users ↔ companies
```

## Getting Started

1. Set your connection string in `appsettings.json`
2. Configure JWT settings (`JWT:Secret`, `JWT:Issuer`, `JWT:Audience`)
3. Run migrations: `dotnet ef database update`
4. Run the project and open `/swagger` to explore the API
