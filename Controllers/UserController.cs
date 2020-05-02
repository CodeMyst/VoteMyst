using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

using VoteMyst.Discord;
using VoteMyst.Database;
using VoteMyst.Database.Models;

namespace VoteMyst.Controllers
{
    public class UserController : Controller
    {
        public class UserDisplay
        {
            public bool IsSelf { get; set; }
            public string Username { get; set; }
            public string DisplayId { get; set; }
            public string PermissionGroup { get; set; }
            public DateTime JoinDate { get; set; }
        }

        private UserProfileBuilder _profileBuilder;

        public UserController(UserProfileBuilder profileBuilder) 
        {
            _profileBuilder = profileBuilder;
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

        public IActionResult Display(string displayId) 
        {
            UserData user = _profileBuilder.FromId(displayId);
            if (user == null)
                return View("NotFound");

            UserData selfUser = _profileBuilder.FromContext(HttpContext);

            // TODO: Make sure the page has the information needed to display the profile
            ViewBag.User = new UserDisplay 
            {
                IsSelf = user.DisplayId == selfUser.DisplayId,
                Username = user.Username,
                DisplayId = user.DisplayId,
                PermissionGroup = user.PermissionLevel.ToString(),
                JoinDate = user.JoinDate
            };
            return View(nameof(Display));
        }

        public IActionResult DisplaySelf()
        {
            if (!User.Identity.IsAuthenticated)
                return Forbid();

            UserData selfUser = _profileBuilder.FromContext(HttpContext);
            return Display(selfUser.DisplayId);
        }

        public IActionResult BanUser(int id)
        {
            throw new NotImplementedException();
        }
    }
}