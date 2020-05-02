using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;

using VoteMyst.Discord;
using VoteMyst.Database;
using Microsoft.AspNetCore.Authorization;

namespace VoteMyst.Controllers
{
    public class UserController : Controller
    {
        public class UserDisplay
        {
            public string DiscordName { get; set; }
            public string DiscordTag { get; set; }
            public string PermissionGroup { get; set; }
            public string AvatarUrl { get; set; }
            public DateTime JoinDate { get; set; }
        }

        private UserDataHelper _userDataHelper;

        public UserController(UserDataHelper userDataHelper) 
        {
            _userDataHelper = userDataHelper;
        }

        public IActionResult Search() 
        {
            return View();
        }
        
        public IActionResult Login()
        {
            return Challenge(new AuthenticationProperties { RedirectUri = "/users/me" });
        }

        public IActionResult Logout()
        {
            Response.Cookies.Delete("DiscordCookie");
            return RedirectToAction("Index", "Home");
        }

        public IActionResult Display(int userId) 
        {
            string oauthToken = HttpContext.GetTokenAsync("access_token").GetAwaiter().GetResult();
            DiscordUser discordUser = new DiscordService(oauthToken).GetUserAsync().GetAwaiter().GetResult();

            // TODO: Check database for user ID and use userId parameter
            var user = _userDataHelper.GetOrCreateUser(discordUser.ID);

            // Make sure the page has the information needed to display the profile
            ViewBag.User = new UserDisplay 
            {
                DiscordName = discordUser.Username,
                DiscordTag = discordUser.Discriminator,
                PermissionGroup = user.PermissionLevel.ToString(),
                AvatarUrl = $"https://cdn.discordapp.com/avatars/{discordUser.ID}/{discordUser.Avatar}.png",
                JoinDate = user.JoinDate
            };
            return View(nameof(Display));
        }

        public IActionResult DisplaySelf()
        {
            // TODO: Fetch self user ID
            return Display(-1);
        }

        public IActionResult BanUser(int id)
        {
            throw new NotImplementedException();
        }
    }
}