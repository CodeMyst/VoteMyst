using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;

using VoteMyst.Discord;

namespace VoteMyst.ViewComponents
{
    public class UserWidgetViewComponent : ViewComponent
    {
        public Task<IViewComponentResult> InvokeAsync() 
        {
            bool loggedIn = User.Identity.IsAuthenticated;
            ViewBag.IsLoggedIn = loggedIn;

            if (loggedIn)
            {
                ViewBag.Username = "TODO:Username";
                ViewBag.Avatar = "https://via.placeholder.com/64";
            }

            return Task.FromResult<IViewComponentResult>(View());
        }
    }
}