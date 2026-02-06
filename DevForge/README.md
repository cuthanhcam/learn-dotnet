# ?? DevForge - Authentication & Authorization System
## Complete Implementation with DDD + Clean Architecture

---

## ?? **OVERVIEW**

This is a **production-ready** authentication and authorization system built with:
- ? **Domain-Driven Design (DDD)**
- ? **Clean Architecture**
- ? **.NET 10 (Latest LTS)**
- ? **CQRS with MediatR**
- ? **Entity Framework Core 10**
- ? **JWT Authentication**
- ? **Role-Based + Permission-Based Authorization**
- ? **Swagger/OpenAPI Documentation**

---

## ??? **PROJECT STRUCTURE**

```
DevForge/
??? src/
?   ??? DevForge.Domain/           # Domain Layer (Business Logic)
?   ?   ??? Entities/              # Aggregate Roots
?   ?   ??? ValueObjects/          # Value Objects
?   ?   ??? Events/                # Domain Events
?   ?   ??? Repositories/          # Repository Interfaces
?   ?   ??? Services/              # Domain Service Interfaces
?   ?   ??? Specifications/        # Business Rules
?   ?   ??? Constants/             # Domain Constants
?   ?
?   ??? DevForge.Application/      # Application Layer (Use Cases)
?   ?   ??? Features/
?   ?   ?   ??? Auth/
?   ?   ?       ??? Commands/      # CQRS Commands
?   ?   ?       ??? DTOs/          # Data Transfer Objects
?   ?   ?       ??? Handlers/      # Command Handlers
?   ?   ?       ??? Validators/    # FluentValidation
?   ?   ??? Common/
?   ?   ?   ??? Behaviors/         # Pipeline Behaviors
?   ?   ?   ??? Interfaces/        # Service Interfaces
?   ?   ?   ??? Models/            # Common Models
?   ?   ??? DependencyInjection.cs
?   ?
?   ??? DevForge.Infrastructure/   # Infrastructure Layer
?   ?   ??? Persistence/
?   ?   ?   ??? Configurations/    # EF Core Configurations
?   ?   ?   ??? Repositories/      # Repository Implementations
?   ?   ?   ??? Seeding/           # Database Seeding
?   ?   ?   ??? Migrations/        # EF Migrations
?   ?   ?   ??? ApplicationDbContext.cs
?   ?   ?   ??? UnitOfWork.cs
?   ?   ??? Services/              # Service Implementations
?   ?   ?   ??? PasswordHasher.cs  # BCrypt
?   ?   ?   ??? JwtService.cs      # JWT Generation
?   ?   ?   ??? TokenGenerator.cs  # Secure Tokens
?   ?   ?   ??? EmailService.cs    # Email Sending
?   ?   ?   ??? TwoFactorService.cs# 2FA (TOTP)
?   ?   ??? DependencyInjection.cs
?   ?
?   ??? DevForge.API/              # Presentation Layer
?       ??? Controllers/           # API Controllers
?       ??? Authorization/         # Custom Authorization
?       ??? Middleware/            # Exception Handling
?       ??? Program.cs             # Application Entry Point
?       ??? appsettings.json       # Configuration
```

---

## ?? **PACKAGES INSTALLED**

### Domain Layer:
- No external dependencies (Pure business logic)

### Application Layer:
- **MediatR 12.4.1** - CQRS pattern
- **FluentValidation 11.11.0** - Input validation
- **FluentValidation.DependencyInjectionExtensions 11.11.0**
- **Mapster 7.4.0** - Object mapping

### Infrastructure Layer:
- **Microsoft.EntityFrameworkCore.SqlServer 10.0.0** - Database
- **BCrypt.Net-Next 4.0.3** - Password hashing
- **Microsoft.AspNetCore.Authentication.JwtBearer 10.0.0** - JWT auth
- **Otp.NET 1.4.0** - Two-factor authentication (TOTP)

### API Layer:
- **Swashbuckle.AspNetCore 6.9.0** - Swagger/OpenAPI
- **Microsoft.EntityFrameworkCore.Design 10.0.0** - Migrations

---

## ?? **QUICK START**

### 1. **Update Connection String** (Optional)
Edit `appsettings.Development.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=DevForgeDb_Dev;Trusted_Connection=true;MultipleActiveResultSets=true;TrustServerCertificate=true"
  }
}
```

### 2. **Apply Migrations & Seed Data**
```powershell
cd src/DevForge.API
dotnet run
```
*Database will be automatically created and seeded on first run.*

### 3. **Access Swagger UI**
Open browser: `https://localhost:5001` or `http://localhost:5000`

### 4. **Test with Default Admin Account**
```
Email: admin@devforge.com
Password: Admin@123456
```

---

## ?? **AUTHENTICATION FEATURES**

### Implemented:
- ? **User Registration** with email confirmation
- ? **Login** with username or email
- ? **JWT Access Tokens** (1 hour expiration)
- ? **Refresh Tokens** (7-30 days, with rotation)
- ? **Password Reset** via email
- ? **Email Confirmation** required
- ? **Change Password** for authenticated users
- ? **Two-Factor Authentication** (TOTP - Google Authenticator compatible)
- ? **Account Lockout** after 5 failed login attempts (15 min)
- ? **Failed Login Tracking**
- ? **IP Address Tracking** for security

### Security Best Practices:
- ? BCrypt password hashing (12 rounds)
- ? Strong password policy (8+ chars, uppercase, lowercase, digit, special char)
- ? Token rotation to prevent replay attacks
- ? Refresh token revocation
- ? Automatic account lockout
- ? Email confirmation required

---

## ??? **AUTHORIZATION FEATURES**

### Role-Based Access Control (RBAC):
- ? Dynamic roles (not hardcoded enum)
- ? Users can have multiple roles
- ? 3 default roles: **Administrator**, **User**, **Moderator**
- ? System roles cannot be modified/deleted

### Permission-Based Access Control:
- ? **17 granular permissions** across 4 categories:
  - **Users:** read, create, update, delete, manage_roles
  - **Roles:** read, create, update, delete, manage_permissions
  - **Permissions:** read, create, update, delete
  - **System:** admin, audit, settings

### Permission Checks:
```csharp
[Authorize(Policy = Permissions.UsersDelete)]
public async Task<IActionResult> DeleteUser(Guid id) { ... }
```

### Administrator Role:
- Full access to all 17 permissions
- Cannot be deactivated
- Cannot modify system roles

---

## ?? **API ENDPOINTS**

### Authentication:
| Method | Endpoint | Description | Auth Required |
|--------|----------|-------------|---------------|
| POST | `/api/auth/register` | Register new user | No |
| POST | `/api/auth/login` | Login | No |
| POST | `/api/auth/refresh-token` | Refresh access token | No |
| POST | `/api/auth/logout` | Logout & revoke token | Yes |
| POST | `/api/auth/change-password` | Change password | Yes |
| POST | `/api/auth/forgot-password` | Request password reset | No |
| POST | `/api/auth/reset-password` | Reset password | No |
| POST | `/api/auth/confirm-email` | Confirm email | No |
| POST | `/api/auth/resend-email-confirmation` | Resend confirmation | No |
| POST | `/api/auth/2fa/enable` | Enable 2FA | Yes |
| POST | `/api/auth/2fa/disable` | Disable 2FA | Yes |
| POST | `/api/auth/2fa/verify` | Verify 2FA code | Yes |
### Health & Monitoring:
| Method | Endpoint | Description | Auth Required |
|--------|----------|-------------|---------------|
| GET | `/health` | Health check (database status) | No |
| GET | `/api/v1/health/ping` | Simple ping endpoint | No |
| GET | `/api/v1/health/info` | API information & uptime | No |

### Rate Limiting:
- **General:** 60 requests per minute per IP
- **Login:** 5 attempts per minute per IP
- **Register:** 3 registrations per hour per IP
- **Response:** 429 Too Many Requests when exceeded
---

## ?? **DATABASE SCHEMA**

### Tables:
1. **Users** - User accounts
2. **Roles** - Dynamic roles
3. **Permissions** - Granular permissions
4. **UserRoles** - Many-to-many (User ? Role)
5. **RolePermissions** - Many-to-many (Role ? Permission)
6. **RefreshTokens** - JWT refresh tokens

### Seeded Data:
- **17 Permissions** (all categories)
- **3 Roles** (Administrator, User, Moderator)
- **1 Admin User** (email confirmed, full access)

---

## ?? **TESTING THE API**

### 1. Register a New User:
```http
POST /api/auth/register
Content-Type: application/json

{
  "username": "johndoe",
  "email": "john@example.com",
  "password": "SecureP@ss123",
  "confirmPassword": "SecureP@ss123",
  "phoneNumber": "+1234567890"
}
```

### 2. Login:
```http
POST /api/auth/login
Content-Type: application/json

{
  "usernameOrEmail": "admin@devforge.com",
  "password": "Admin@123456",
  "rememberMe": true
}
```

**Response:**
```json
{
  "accessToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "refreshToken": "xyz123...",
  "expiresAt": "2024-01-01T12:00:00Z",
  "user": {
    "id": "guid",
    "username": "admin",
    "email": "admin@devforge.com",
    "isActive": true,
    "emailConfirmed": true,
    "roles": ["Administrator"],
    "permissions": ["users.read", "users.create", ...]
  }
}
```

### 3. Use Access Token:
Add header to all authenticated requests:
```
Authorization: Bearer {accessToken}
```

### 4. Refresh Token:
```http
POST /api/auth/refresh-token
Content-Type: application/json

{
  "refreshToken": "xyz123..."
}
```

---

## ?? **CONFIGURATION**

### JWT Settings (`appsettings.json`):
```json
{
  "Jwt": {
    "Secret": "YourSuperSecretKeyThatIsAtLeast32CharactersLong",
    "Issuer": "DevForge",
    "Audience": "DevForgeUsers",
    "ExpirationInHours": 1
  }
}
```

?? **IMPORTANT:** Change `Jwt:Secret` in production!

### Database (`appsettings.json`):
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=DevForgeDb;..."
  }
}
```

---

## ?? **DOMAIN MODEL**

### Entities:
- **User** (Aggregate Root) - 350+ lines of business logic
- **Role** (Aggregate Root) - Dynamic role management
- **Permission** (Aggregate Root) - Granular permissions
- **RefreshToken** (Aggregate Root) - Token lifecycle
- **UserRole** - Join entity
- **RolePermission** - Join entity

### Value Objects:
- **Email** - With regex validation
- **Username** - 3-50 chars, alphanumeric
- **Password** - Strong policy validation
- **PasswordHash** - Hashed password
- **PhoneNumber** - International format

### Domain Events (19 total):
- UserCreatedEvent, UserActivatedEvent, UserDeactivatedEvent
- UserPasswordChangedEvent, UserProfileUpdatedEvent
- UserEmailConfirmedEvent, UserTwoFactorEnabledEvent
- UserLoginFailedEvent, UserLockedOutEvent, UserLoggedInEvent
- UserRoleAssignedEvent, UserRoleRemovedEvent
- RoleCreatedEvent, RoleUpdatedEvent
- PermissionAddedToRoleEvent, PermissionRemovedFromRoleEvent
- PermissionCreatedEvent, PermissionUpdatedEvent
- RefreshTokenCreatedEvent, RefreshTokenRevokedEvent

---

## ?? **CLEAN ARCHITECTURE LAYERS**

### Dependency Flow:
```
API (Presentation)
   ?
Application (Use Cases)
   ?
Domain (Business Logic) ? Infrastructure (Implementation)
```

### Key Principles:
- ? Domain has ZERO dependencies
- ? Repository interfaces in Domain
- ? Repository implementations in Infrastructure
- ? Dependency Inversion Principle
- ? Separation of Concerns
- ? Testability (each layer can be unit tested)

---

## ?? **SECURITY CHECKLIST**

- ? Passwords hashed with BCrypt (12 rounds)
- ? JWT tokens signed with HS256
- ? Refresh token rotation
- ? Account lockout after 5 failed attempts
- ? Email confirmation required
- ? Two-factor authentication support
- ? IP address tracking
- ? Token expiration
- ? CORS configuration
- ? HTTPS redirection
- ?? Change JWT Secret in production
- ?? Enable RequireHttpsMetadata in production
- ?? Configure proper CORS policy

---

## ?? **ADDITIONAL FEATURES TO IMPLEMENT**

While the current implementation is production-ready for authentication/authorization, you may want to add:

1. **Email Service** - Currently uses fake implementation
   - Integrate SendGrid, AWS SES, or SMTP
   
2. **User Management APIs** - CRUD operations for users
   - GET /api/users
   - GET /api/users/{id}
   - PUT /api/users/{id}
   - DELETE /api/users/{id}

3. **Role Management APIs** - CRUD for roles
   - GET /api/roles
   - POST /api/roles
   - PUT /api/roles/{id}
   - DELETE /api/roles/{id}

4. **Audit Logging** - Track all important actions
   
5. **Rate Limiting** - Prevent brute force attacks

6. **Caching** - Redis for performance

7. **Health Checks** - Monitor application status

---

## ?? **USEFUL COMMANDS**

### Create New Migration:
```powershell
dotnet ef migrations add MigrationName --project src\DevForge.Infrastructure --startup-project src\DevForge.API
```

### Apply Migrations:
```powershell
dotnet ef database update --project src\DevForge.Infrastructure --startup-project src\DevForge.API
```

### Remove Last Migration:
```powershell
dotnet ef migrations remove --project src\DevForge.Infrastructure --startup-project src\DevForge.API
```

### Build Solution:
```powershell
dotnet build
```

### Run API:
```powershell
cd src\DevForge.API
dotnet run
```

---

## ?? **DOCUMENTATION**

- **Swagger UI**: Available at root URL when running in Development
- **Domain README**: `src/DevForge.Domain/README.md` - Complete domain documentation
- **API Documentation**: Auto-generated from XML comments

---

## ? **PRODUCTION CHECKLIST**

Before deploying to production:

1. ? Change JWT Secret to secure random string
2. ? Update connection string
3. ? Enable HTTPS only (`RequireHttpsMetadata = true`)
4. ? Configure proper CORS policy (not AllowAll)
5. ? Set up proper email service
6. ? Configure logging (Serilog, Application Insights, etc.)
7. ? Set up monitoring and health checks
8. ? Enable rate limiting
9. ? Review and adjust token expiration times
10. ? Set up database backups
11. ? Configure CI/CD pipeline
12. ? Security audit

---

## ?? **CONGRATULATIONS!**

You now have a **fully functional, production-ready authentication and authorization system** built with:
- Clean Architecture
- Domain-Driven Design
- CQRS Pattern
- Latest .NET 10 LTS
- Industry best practices

The system includes:
- 12 authentication endpoints
- JWT + Refresh tokens
- Role-based + Permission-based authorization
- Two-factor authentication
- Account lockout protection
- Complete Swagger documentation
- Database migrations and seeding

**Start the API and test it via Swagger!** ??

---

## ?? **SUPPORT**

For questions or issues, please refer to:
- Domain documentation: `src/DevForge.Domain/README.md`
- Swagger UI: Run the API and navigate to root URL
- Code comments: Extensive inline documentation

---

**Built with ?? using Clean Architecture + DDD principles**
