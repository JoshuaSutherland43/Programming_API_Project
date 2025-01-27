# C# API Application with Identity, JWT, and Direct Database Queries

This repository contains a robust **C# API** application developed using ASP.NET Core. The API leverages **Identity** for authentication and user management, **JWT tokens** for secure communication, **Entity Framework Core** with migrations for database management, and direct SQL queries for optimized database access. The API integrates seamlessly with a main project, enabling bi-directional communication.

---

## 📂 Project Features

### 🔐 **Authentication & Authorization**
- **ASP.NET Core Identity** for user registration, login, and role management.
- **JWT tokens** for secure authentication and authorization.
- Role-based access control to manage permissions effectively.

### 🗂️ **Database Management**
- **Entity Framework Core** for database migrations and schema evolution.
- Support for direct SQL queries for optimized and custom database interactions.
- Secure handling of sensitive data using parameterized queries.

### 🔄 **Integration**
- Designed to be called by a **main project** for various functionalities.
- Provides APIs that enable communication with the main project and also consumes APIs exposed by the main project.

---

## 🛠️ Technologies Used

- **C#**
- **ASP.NET Core Web API**
- **Entity Framework Core** (Code-First Migrations)
- **SQL Server**
- **JWT (JSON Web Tokens)** for Authentication
- **Newtonsoft.Json** for JSON serialization
- **Postman** (or similar tools) for API testing

---

## 📁 Directory Structure

```plaintext
📦 CSharpAPIProject
├── Controllers/
│   ├── AccountController.cs         # User registration and authentication
│   ├── RolesController.cs           # Role management
│   ├── DatabaseQueryController.cs   # Direct SQL query endpoints
├── Models/
│   ├── UserModel.cs                 # User data model
│   ├── RoleModel.cs                 # Role data model
├── Data/
│   ├── ApplicationDbContext.cs      # Database context (EF Core)
│   ├── Migrations/                  # EF Core migration files
├── Helpers/
│   ├── JwtTokenHelper.cs            # JWT generation and validation
├── Program.cs                       # API startup and configuration
├── appsettings.json                 # App configuration (e.g., connection strings)
└── README.md                        # Project documentation
