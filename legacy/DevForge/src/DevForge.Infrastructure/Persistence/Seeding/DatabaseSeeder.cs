using DevForge.Domain.Constants;
using DevForge.Domain.Entities;
using DevForge.Domain.Services;
using DevForge.Domain.ValueObjects;
using DevForge.Infrastructure.Persistence;

namespace DevForge.Infrastructure.Persistence.Seeding
{
    public static class DatabaseSeeder
    {
        public static async Task SeedAsync(ApplicationDbContext context, IPasswordHasher passwordHasher)
        {
            // Seed Permissions
            if (!context.Permissions.Any())
            {
                var permissions = new List<Permission>
                {
                    // User Permissions
                    Permission.Create(Permissions.UsersRead, "Read user information", Permissions.Categories.Users),
                    Permission.Create(Permissions.UsersCreate, "Create new users", Permissions.Categories.Users),
                    Permission.Create(Permissions.UsersUpdate, "Update user information", Permissions.Categories.Users),
                    Permission.Create(Permissions.UsersDelete, "Delete users", Permissions.Categories.Users),
                    Permission.Create(Permissions.UsersManageRoles, "Manage user roles", Permissions.Categories.Users),
                    
                    // Role Permissions
                    Permission.Create(Permissions.RolesRead, "Read role information", Permissions.Categories.Roles),
                    Permission.Create(Permissions.RolesCreate, "Create new roles", Permissions.Categories.Roles),
                    Permission.Create(Permissions.RolesUpdate, "Update role information", Permissions.Categories.Roles),
                    Permission.Create(Permissions.RolesDelete, "Delete roles", Permissions.Categories.Roles),
                    Permission.Create(Permissions.RolesManagePermissions, "Manage role permissions", Permissions.Categories.Roles),
                    
                    // Permission Permissions
                    Permission.Create(Permissions.PermissionsRead, "Read permission information", Permissions.Categories.Permissions),
                    Permission.Create(Permissions.PermissionsCreate, "Create new permissions", Permissions.Categories.Permissions),
                    Permission.Create(Permissions.PermissionsUpdate, "Update permission information", Permissions.Categories.Permissions),
                    Permission.Create(Permissions.PermissionsDelete, "Delete permissions", Permissions.Categories.Permissions),
                    
                    // System Permissions
                    Permission.Create(Permissions.SystemAdmin, "Full system administration access", Permissions.Categories.System),
                    Permission.Create(Permissions.SystemAudit, "View system audit logs", Permissions.Categories.System),
                    Permission.Create(Permissions.SystemSettings, "Manage system settings", Permissions.Categories.System)
                };

                await context.Permissions.AddRangeAsync(permissions);
                await context.SaveChangesAsync();
            }

            // Seed Roles
            if (!context.Roles.Any())
            {
                var allPermissions = context.Permissions.ToList();

                // Administrator Role - Full Access
                var adminRole = Role.Create(Roles.Administrator, "Full system access with all permissions", true);
                foreach (var permission in allPermissions)
                {
                    adminRole.AddPermission(permission);
                }

                // User Role - Basic Access
                var userRole = Role.Create(Roles.User, "Basic user access", true);
                var userPermissions = allPermissions.Where(p => 
                    p.Name == Permissions.UsersRead).ToList();
                foreach (var permission in userPermissions)
                {
                    userRole.AddPermission(permission);
                }

                // Moderator Role - Content Management
                var moderatorRole = Role.Create(Roles.Moderator, "Content moderation access", false);
                var moderatorPermissions = allPermissions.Where(p => 
                    p.Name == Permissions.UsersRead ||
                    p.Name == Permissions.UsersUpdate ||
                    p.Name == Permissions.RolesRead).ToList();
                foreach (var permission in moderatorPermissions)
                {
                    moderatorRole.AddPermission(permission);
                }

                await context.Roles.AddRangeAsync(adminRole, userRole, moderatorRole);
                await context.SaveChangesAsync();
            }

            // Seed Admin User
            if (!context.Users.Any())
            {
                var adminRole = context.Roles.First(r => r.Name == Roles.Administrator);

                var username = Username.Create("admin");
                var email = Email.Create("admin@devforge.com");
                var password = Password.Create("Admin@123456");
                var passwordHash = passwordHasher.HashPassword(password);

                var adminUser = User.Create(username, email, passwordHash);
                adminUser.AssignRole(adminRole.Id);
                
                // Confirm email for admin
                var token = Guid.NewGuid().ToString();
                adminUser.GenerateEmailConfirmationToken(token);
                adminUser.ConfirmEmail(token);

                await context.Users.AddAsync(adminUser);
                await context.SaveChangesAsync();
            }
        }
    }
}
