using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;

using VoteMyst.Database;

namespace VoteMyst.Maintenance
{
    /// <summary>
    /// Handles the request in case the maintenance mode is activated.
    /// </summary>
    public class MaintenanceModeMiddleware
    {
        private readonly RequestDelegate _next;

        public MaintenanceModeMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context, Configuration configuration, UserProfileBuilder profileBuilder)
        {
            // Check if the site is currently in maintenance mode.
            bool isMaintenance = bool.Parse(configuration.GetValue(MaintenanceModeConstants.ConfigurationKey, bool.FalseString));

            if (context.Request.Path != MaintenanceModeConstants.PagePath)
            {
                // Only further check access rights if the user is not on the maintenance page.
                bool isUserSiteAdmin = profileBuilder.FromPrincipal(context.User).Permissions.HasFlag(GlobalPermissions.SiteAdministrator);

                // Tell the rest of the site if maintenance mode is active.
                context.Items[MaintenanceModeConstants.ConfigurationKey] = isMaintenance;

                // If the maintenance is active and the user does not have the access rights to the rest of the page, redirect him to the maintenance page.
                if (isMaintenance && !isUserSiteAdmin)
                {
                    context.Response.Redirect(MaintenanceModeConstants.PagePath);
                }
            }
            else
            {
                // If the user is already on the maintenance page, but the maintenance is not enabled, redirect him to the homepage.
                if (!isMaintenance)
                {
                    context.Response.Redirect("/");
                }
            }

            await _next(context);
        }

        /// <summary>
        /// Checks if the maintenance mode is active and enabled in the given <see cref="HttpContext"/>.
        /// </summary>
        public static bool CheckActiveAndEnabled(HttpContext context)
        {
            return context.Items[MaintenanceModeConstants.ConfigurationKey] is bool maintenanceActive && maintenanceActive;
        }
    }
}
