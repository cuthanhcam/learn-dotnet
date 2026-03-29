# 🚀 DevForge - Authentication & Authorization System

> Production-ready Authentication & Authorization System built with **DDD + Clean Architecture + .NET 10**

---

## 📌 Overview

DevForge is a **fully-featured authentication & authorization system** designed with modern best practices:

* Domain-Driven Design (DDD)
* Clean Architecture
* CQRS (MediatR)
* Entity Framework Core 10
* JWT Authentication + Refresh Tokens
* Role-Based & Permission-Based Authorization
* Two-Factor Authentication (TOTP)
* Swagger / OpenAPI

---

## 🏗️ Architecture

### Clean Architecture Layers

```
API (Presentation)
   ↓
Application (Use Cases)
   ↓
Domain (Business Logic) ← Infrastructure (External Concerns)
```

### Principles

* Domain has **zero dependencies**
* Dependency Inversion enforced
* Clear separation of concerns
* Fully testable design

---

## 📂 Project Structure

```
DevForge/
└── src/
    ├── DevForge.Domain/           # Core business logic
    ├── DevForge.Application/      # Use cases (CQRS)
    ├── DevForge.Infrastructure/   # External implementations
    └── DevForge.API/              # Presentation layer
```

### Highlights

* **Domain** → Entities, ValueObjects, Events
* **Application** → Commands, Handlers, Validators
* **Infrastructure** → EF Core, JWT, Services
* **API** → Controllers, Middleware, Authorization

---

## ⚙️ Tech Stack

### Application

* MediatR (CQRS)
* FluentValidation
* Mapster

### Infrastructure

* Entity Framework Core 10
* BCrypt (password hashing)
* JWT Bearer Authentication
* Otp.NET (2FA)

### API

* Swagger / OpenAPI

---

## 🚀 Quick Start

### 1. Configure Database (optional)

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=DevForgeDb_Dev;Trusted_Connection=true"
}
```

---

### 2. Run the Application

```bash
cd src/DevForge.API
dotnet run
```

Database will be auto-created & seeded

---

### 3. Access Swagger

```
https://localhost:5001
```

---

### 4. Default Admin Account

```
Email: admin@devforge.com
Password: Admin@123456
```

---

## 🔐 Authentication Features

* User Registration + Email Confirmation
* Login (username or email)
* JWT Access Token (1h)
* Refresh Token (rotation supported)
* Password Reset via Email
* Change Password
* Two-Factor Authentication (TOTP)
* Account Lockout (5 failed attempts)
* IP Tracking & Login Monitoring

### Security

* BCrypt hashing (12 rounds)
* Strong password policy
* Token rotation & revocation
* Email verification required

---

## 🛡️ Authorization

### RBAC (Role-Based)

* Dynamic roles (not enum-based)
* Multiple roles per user
* Default roles:

  * Administrator
  * User
  * Moderator

---

### Permission-Based

17 granular permissions across:

* Users
* Roles
* Permissions
* System

### Example

```csharp
[Authorize(Policy = Permissions.UsersDelete)]
```

---

## 🌐 API Endpoints

### Authentication

| Method | Endpoint                    | Description     |
| ------ | --------------------------- | --------------- |
| POST   | `/api/auth/register`        | Register        |
| POST   | `/api/auth/login`           | Login           |
| POST   | `/api/auth/refresh-token`   | Refresh token   |
| POST   | `/api/auth/logout`          | Logout          |
| POST   | `/api/auth/change-password` | Change password |
| POST   | `/api/auth/forgot-password` | Forgot password |
| POST   | `/api/auth/reset-password`  | Reset password  |
| POST   | `/api/auth/confirm-email`   | Confirm email   |
| POST   | `/api/auth/2fa/enable`      | Enable 2FA      |
| POST   | `/api/auth/2fa/verify`      | Verify 2FA      |

---

### Health Check

| Endpoint              | Description |
| --------------------- | ----------- |
| `/health`             | DB health   |
| `/api/v1/health/ping` | Ping        |
| `/api/v1/health/info` | Info        |

---

## 🗄️ Database

### Tables

* Users
* Roles
* Permissions
* UserRoles
* RolePermissions
* RefreshTokens

### Seed Data

* 17 permissions
* 3 roles
* 1 admin user

---

## 🧪 Example Usage

### Login

```http
POST /api/auth/login
```

```json
{
  "usernameOrEmail": "admin@devforge.com",
  "password": "Admin@123456"
}
```

---

### Use Token

```
Authorization: Bearer {accessToken}
```

---

## ⚡ Configuration

### JWT

```json
"Jwt": {
  "Secret": "YOUR_SECRET_KEY",
  "Issuer": "DevForge",
  "Audience": "DevForgeUsers",
  "ExpirationInHours": 1
}
```

⚠️ **IMPORTANT:** Change secret in production

---

## 🧠 Domain Model

### Entities

* User (Aggregate Root)
* Role
* Permission
* RefreshToken

### Value Objects

* Email
* Username
* Password
* PhoneNumber

### Domain Events

* UserCreated
* UserLockedOut
* UserLoggedIn
* RoleUpdated
* PermissionAssigned
  *(19 events total)*

---

## 🔒 Security Checklist

* Password hashing (BCrypt)
* JWT signing
* Refresh token rotation
* Account lockout
* Email confirmation
* 2FA support
* HTTPS
* CORS

---

## 🧩 Future Improvements

* Real Email Service (SendGrid / SES)
* User Management APIs
* Role Management APIs
* Audit Logging
* Redis Caching
* Advanced Rate Limiting

---

## 🛠️ Useful Commands

### Migration

```bash
dotnet ef migrations add Init
dotnet ef database update
```

### Run

```bash
dotnet run
```

---

## 🚀 Production Checklist

* Change JWT Secret
* Enable HTTPS
* Configure CORS
* Setup logging & monitoring
* Enable rate limiting
* Setup backups
* CI/CD pipeline

---

## 📖 Documentation

* Swagger UI (auto-generated)
* Domain docs: `DevForge.Domain/README.md`

---

## 🎉 Summary

DevForge provides:

* Complete authentication system
* JWT + Refresh Token
* RBAC + Permission system
* 2FA support
* Clean Architecture + DDD
* Production-ready foundation

---

## 💬 Support

* Check Swagger UI
* Read domain documentation
* Explore inline code comments

---

**Built with ❤️ using Clean Architecture & DDD**
