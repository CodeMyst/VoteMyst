using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using VoteMyst.Database;

namespace VoteMyst
{
    public static class MaintenanceModeMiddlewareExtensions
    {
        public static IApplicationBuilder UseMaintenanceMode(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<MaintenanceModeMiddleware>();
        }
    }

    public class MaintenanceModeMiddleware
    {
        private readonly RequestDelegate _next;

        public MaintenanceModeMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context, Configuration configuration, IWebHostEnvironment environment, UserProfileBuilder profileBuilder)
        {
            if (bool.Parse(configuration.GetValue("maintenanceMode", "false"))
                && (profileBuilder.FromPrincipal(context.User).Permissions & GlobalPermissions.SiteAdministrator) == 0)
            {
                string content = await File.ReadAllTextAsync(Path.Combine(environment.WebRootPath, "maintenance.html"));
                await context.Response.WriteAsync(content);
            }
            else
            {
                await _next(context);
            }
        }
    }
}
