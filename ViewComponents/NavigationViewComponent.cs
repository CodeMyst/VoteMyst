using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;

using VoteMyst.Database;
using VoteMyst.Database.Models;

namespace VoteMyst.ViewComponents
{
    public class NavigationViewComponent : ViewComponent
    {
        private readonly UserProfileBuilder _profileBuilder;
        private readonly DatabaseHelperProvider _helpers;

        public NavigationViewComponent(UserProfileBuilder profileBuilder, DatabaseHelperProvider helpers)
        {
            _profileBuilder = profileBuilder;
            _helpers = helpers;
        }

        public Task<IViewComponentResult> InvokeAsync() 
        {
            Event[] events = _helpers.Events.GetCurrentEvents();
            UserData user = _profileBuilder.FromPrincipal(UserClaimsPrincipal);

            ViewBag.HasCurrentEvent = events.Length > 0;
            ViewBag.IsAdmin = user.IsAdmin();
            
            return Task.FromResult<IViewComponentResult>(View());
        }
    }
}