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
            Event[] planned = _helpers.Events.GetVisiblePlannedEvents();
            UserData user = _profileBuilder.FromPrincipal(UserClaimsPrincipal);

            ViewBag.HasBrowseableEvents = events.Length > 0 || planned.Length > 0;
            ViewBag.HasCurrentEvent = events.Length > 0;

            ViewBag.CanCreateEvents = user.HasPermission(Permissions.CreateEvents);
            ViewBag.CanModifyUser = user.HasPermission(Permissions.ModifyUsers);
            ViewBag.HasOutlineEntries = ViewBag.CanCreateEvents || ViewBag.CanModifyUser;
            
            return Task.FromResult<IViewComponentResult>(View());
        }
    }
}