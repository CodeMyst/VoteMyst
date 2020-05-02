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
        private readonly EventHelper _eventHelper;
        private readonly UserProfileBuilder _profileBuilder;

        public NavigationViewComponent(EventHelper eventHelper, UserProfileBuilder profileBuilder)
        {
            _eventHelper = eventHelper;
            _profileBuilder = profileBuilder;
        }

        public Task<IViewComponentResult> InvokeAsync() 
        {
            Event[] events = _eventHelper.GetCurrentEvents();
            UserData user = _profileBuilder.FromContext(HttpContext);

            ViewBag.HasCurrentEvent = events.Length > 0;
            ViewBag.IsAdmin = user.PermissionLevel == Permissions.Admin;
            
            return Task.FromResult<IViewComponentResult>(View());
        }
    }
}