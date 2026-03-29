using DevForge.API.Authorization;
using Microsoft.AspNetCore.Authorization;
using System.Reflection;

namespace DevForge.API.Extensions
{
    /// <summary>
    /// Extension methods for authorization configuration
    /// </summary>
    public static class AuthorizationExtensions
    {
        /// <summary>
        /// Adds dynamic permission-based authorization policies
        /// </summary>
        public static IServiceCollection AddPermissionPolicies(this IServiceCollection services)
        {
            services.AddSingleton<IAuthorizationHandler, PermissionAuthorizationHandler>();
            
            services.AddAuthorization(options =>
            {
                // Dynamically register policies for all permissions
                var permissionType = typeof(DevForge.Domain.Constants.Permissions);
                var permissionFields = permissionType.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)
                    .Where(f => f.IsLiteral && !f.IsInitOnly && f.FieldType == typeof(string));

                foreach (var field in permissionFields)
                {
                    var permissionValue = field.GetValue(null)?.ToString();
                    if (!string.IsNullOrEmpty(permissionValue))
                    {
                        options.AddPolicy(permissionValue, policy =>
                            policy.Requirements.Add(new PermissionRequirement(permissionValue)));
                    }
                }
            });

            return services;
        }
    }
}
