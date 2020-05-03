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
    public class AvatarViewComponent : ViewComponent
    {
        private readonly UserProfileBuilder _profileBuilder;

        public AvatarViewComponent(UserProfileBuilder profileBuilder)
        {
            _profileBuilder = profileBuilder;
        }

        public Task<IViewComponentResult> InvokeAsync(UserData user) 
        {
            ViewBag.AvatarUrl = _profileBuilder.GetAvatarUrl(user, out string initials);
            ViewBag.AvatarInitials = initials;

            return Task.FromResult<IViewComponentResult>(View());
        }
    }
}