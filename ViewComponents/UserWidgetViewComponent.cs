using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;

using VoteMyst.Discord;
using VoteMyst.Database;
using VoteMyst.Database.Models;


namespace VoteMyst.ViewComponents
{
    public class UserWidgetViewComponent : ViewComponent
    {
        private readonly UserProfileBuilder _profileBuilder;
        private readonly IWebHostEnvironment _environment;

        public UserWidgetViewComponent(UserProfileBuilder profileBuilder, IWebHostEnvironment environment)
        {
            _profileBuilder = profileBuilder;
            _environment = environment;
        }

        public Task<IViewComponentResult> InvokeAsync() 
        {
            bool loggedIn = User.Identity.IsAuthenticated;
            ViewBag.IsLoggedIn = loggedIn;

            if (loggedIn)
            {
                ViewBag.User = _profileBuilder.FromContext(HttpContext);
            }

            return Task.FromResult<IViewComponentResult>(View());
        }
    }
}