using Microsoft.AspNetCore.Builder;

namespace VoteMyst.Maintenance
{
    /// <summary>
    /// Provides the extension method to include the maintenance mode middleware in the application.
    /// </summary>
    public static class MaintenanceModeMiddlewareExtensions
    {
        /// <summary>
        /// Inserts the maintenance mode middleware into the application. If the maintenance mode is activated in the configuration, non-admin users will no longer have access to the site.
        /// </summary>
        public static IApplicationBuilder UseMaintenanceMode(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<MaintenanceModeMiddleware>();
        }
    }
}
