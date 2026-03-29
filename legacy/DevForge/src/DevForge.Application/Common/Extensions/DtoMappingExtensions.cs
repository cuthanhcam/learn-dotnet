using DevForge.Application.Features.Auth.DTOs;
using DevForge.Domain.Entities;

namespace DevForge.Application.Common.Extensions
{
    /// <summary>
    /// Extension methods for mapping domain entities to DTOs
    /// </summary>
    public static class DtoMappingExtensions
    {
        /// <summary>
        /// Maps a User entity to UserDto
        /// </summary>
        /// <param name="user">The user entity</param>
        /// <param name="roles">User's roles</param>
        /// <param name="permissions">User's permissions</param>
        /// <returns>UserDto with all user information</returns>
        public static UserDto ToDto(
            this User user, 
            IEnumerable<string> roles, 
            IEnumerable<string> permissions)
        {
            if (user == null) 
                throw new ArgumentNullException(nameof(user));

            return new UserDto(
                Id: user.Id,
                Username: user.Username.Value,
                Email: user.Email.Value,
                PhoneNumber: user.PhoneNumber?.Value,
                IsActive: user.IsActive,
                EmailConfirmed: user.EmailConfirmed,
                PhoneNumberConfirmed: user.PhoneNumberConfirmed,
                TwoFactorEnabled: user.TwoFactorEnabled,
                CreatedAt: user.CreatedAt,
                LastLoginAt: user.LastLoginAt,
                Roles: roles.ToList(),
                Permissions: permissions.ToList()
            );
        }

        /// <summary>
        /// Maps a User entity to UserProfileDto
        /// </summary>
        /// <param name="user">The user entity</param>
        /// <param name="roles">User's roles as RoleDto list</param>
        /// <param name="permissions">User's permissions as PermissionDto list</param>
        /// <returns>UserProfileDto</returns>
        public static UserProfileDto ToProfileDto(
            this User user,
            List<RoleDto> roles,
            List<PermissionDto> permissions)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            return new UserProfileDto(
                Id: user.Id,
                Username: user.Username.Value,
                Email: user.Email.Value,
                PhoneNumber: user.PhoneNumber?.Value,
                IsActive: user.IsActive,
                EmailConfirmed: user.EmailConfirmed,
                PhoneNumberConfirmed: user.PhoneNumberConfirmed,
                TwoFactorEnabled: user.TwoFactorEnabled,
                CreatedAt: user.CreatedAt,
                UpdatedAt: user.UpdatedAt,
                LastLoginAt: user.LastLoginAt,
                AccessFailedCount: user.AccessFailedCount,
                LockoutEnd: user.LockoutEnd,
                LockoutEnabled: user.LockoutEnabled,
                Roles: roles,
                Permissions: permissions,
                SecurityInfo: new UserSecurityInfoDto(
                    HasTwoFactorEnabled: user.TwoFactorEnabled,
                    HasEmailConfirmed: user.EmailConfirmed,
                    HasPhoneConfirmed: user.PhoneNumberConfirmed,
                    FailedLoginAttempts: user.AccessFailedCount,
                    IsLockedOut: user.IsLockedOut(),
                    LockoutEndDate: user.LockoutEnd,
                    LastPasswordChangedAt: null // Can add this field to User entity if needed
                )
            );
        }

        /// <summary>
        /// Maps a Role entity to RoleDto (simple version)
        /// </summary>
        /// <param name="role">The role entity</param>
        /// <returns>RoleDto</returns>
        public static RoleDto ToDto(this Role role)
        {
            if (role == null)
                throw new ArgumentNullException(nameof(role));

            return new RoleDto(
                Id: role.Id,
                Name: role.Name,
                Description: role.Description
            );
        }

        /// <summary>
        /// Maps a Role entity to RoleDetailDto with permissions
        /// </summary>
        /// <param name="role">The role entity</param>
        /// <param name="permissions">Role's permissions</param>
        /// <returns>RoleDetailDto</returns>
        public static RoleDetailDto ToDetailDto(
            this Role role,
            IEnumerable<PermissionDto> permissions)
        {
            if (role == null)
                throw new ArgumentNullException(nameof(role));

            return new RoleDetailDto(
                Id: role.Id,
                Name: role.Name,
                Description: role.Description,
                IsSystemRole: role.IsSystemRole,
                CreatedAt: role.CreatedAt,
                UpdatedAt: null, // Can add UpdatedAt to Role entity if needed
                Permissions: permissions.ToList()
            );
        }

        /// <summary>
        /// Maps a Permission entity to PermissionDto
        /// </summary>
        /// <param name="permission">The permission entity</param>
        /// <returns>PermissionDto</returns>
        public static PermissionDto ToDto(this Permission permission)
        {
            if (permission == null)
                throw new ArgumentNullException(nameof(permission));

            return new PermissionDto(
                Id: permission.Id,
                Name: permission.Name,
                Description: permission.Description,
                Category: permission.Category
            );
        }

        /// <summary>
        /// Creates an AuthResponse with user and tokens
        /// </summary>
        /// <param name="user">The user entity</param>
        /// <param name="accessToken">JWT access token</param>
        /// <param name="refreshToken">Refresh token</param>
        /// <param name="expiresAt">Token expiration time</param>
        /// <param name="roles">User's roles</param>
        /// <param name="permissions">User's permissions</param>
        /// <returns>Complete AuthResponse</returns>
        public static AuthResponse ToAuthResponse(
            this User user,
            string accessToken,
            string refreshToken,
            DateTime expiresAt,
            IEnumerable<string> roles,
            IEnumerable<string> permissions)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            return new AuthResponse(
                AccessToken: accessToken,
                RefreshToken: refreshToken,
                ExpiresAt: expiresAt,
                User: user.ToDto(roles, permissions)
            );
        }
    }
}
