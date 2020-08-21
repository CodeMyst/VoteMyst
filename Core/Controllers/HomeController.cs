using System;

using Microsoft.AspNetCore.Mvc;

using VoteMyst.Database;

namespace VoteMyst.Controllers
{
    /// <summary>
    /// Provides the homepage for the site.
    /// </summary>
    public class HomeController : VoteMystController
    {
        public HomeController(IServiceProvider serviceProvider) : base(serviceProvider) { }

        /// <summary>
        /// The index page for the site.
        /// </summary>
        [Route("")]
        public IActionResult Index()
        {
            return View(FeaturedEvents.Generate(DatabaseHelpers));
        }
    }
}