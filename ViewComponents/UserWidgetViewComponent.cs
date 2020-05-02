using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;

using VoteMyst.Discord;
using VoteMyst.Database;
using VoteMyst.Database.Models;

namespace VoteMyst.ViewComponents
{
    public class UserWidgetViewComponent : ViewComponent
    {
        private readonly UserProfileBuilder _profileBuilder;

        public UserWidgetViewComponent(UserProfileBuilder profileBuilder)
        {
            _profileBuilder = profileBuilder;
        }

        public Task<IViewComponentResult> InvokeAsync() 
        {
            bool loggedIn = User.Identity.IsAuthenticated;
            ViewBag.IsLoggedIn = loggedIn;

            if (loggedIn)
            {
                UserData user = _profileBuilder.FromContext(HttpContext);

                ViewBag.Username = user.Username;
                ViewBag.DisplayId = user.DisplayId;
            }

            return Task.FromResult<IViewComponentResult>(View());
        }
    }
}