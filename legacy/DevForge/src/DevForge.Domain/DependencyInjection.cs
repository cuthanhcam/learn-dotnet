using DevForge.Domain.Services;
using Microsoft.Extensions.DependencyInjection;

namespace DevForge.Domain
{
    /// <summary>
    /// Domain layer dependency injection registration
    /// </summary>
    public static class DependencyInjection
    {
        /// <summary>
        /// Registers domain services into the service collection
        /// </summary>
        /// <param name="services">The service collection</param>
        /// <returns>The service collection for chaining</returns>
        public static IServiceCollection AddDomain(this IServiceCollection services)
        {
            // Domain Services - Business logic that doesn't naturally fit within entities
            services.AddScoped<IUserAuthenticationService, UserAuthenticationService>();
            services.AddScoped<RefreshTokenService>();
            services.AddScoped<RolePermissionService>();

            return services;
        }
    }
}
