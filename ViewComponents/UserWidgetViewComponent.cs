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
        public async Task<IViewComponentResult> InvokeAsync() 
        {
            ViewBag.IsLoggedIn = User.Identity.IsAuthenticated;
            if (User.Identity.IsAuthenticated) 
            {
                string oauthToken = await HttpContext.GetTokenAsync("access_token");
                DiscordUser discordUser = await new DiscordService(oauthToken).GetUserAsync();
                ViewBag.DiscordUser = discordUser;
            }

            return View();
        }
    }
}