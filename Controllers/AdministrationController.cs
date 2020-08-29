using System;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

using VoteMyst.Database;
using VoteMyst.Maintenance;
using VoteMyst.Authorization;

namespace VoteMyst.Controllers
{
    /// <summary>
    /// Provides the administration panel for the site.
    /// </summary>
    public class AdministrationController : VoteMystController
    {
        public enum Operation
        {
            Activate,
            Deactivate
        }

        private Configuration _configuration;

        public AdministrationController(IServiceProvider serviceProvider) : base(serviceProvider)
        {
            _configuration = serviceProvider.GetService<Configuration>();
        }

        /// <summary>
        /// The index page for the site.
        /// </summary>
        [Route("administration")]
        [RequireGlobalPermission(GlobalPermissions.SiteAdministrator)]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [Route("administration/maintenance/{operation}")]
        public IActionResult ProcessMaintenanceOperation(Operation operation)
        {
            _configuration[MaintenanceModeConstants.ConfigurationKey] = (operation == Operation.Activate).ToString();

            return RedirectToAction("Index");
        }
    }
}