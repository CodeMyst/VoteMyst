using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;

using VoteMyst.Discord;
using VoteMyst.Database;
using VoteMyst.Database.Models;

namespace VoteMyst.Controllers
{
    public class UserController : Controller
    {
        public class UserDisplay
        {
            public string Username { get; set; }
            public string PermissionGroup { get; set; }
            public string Avatar { get; set; }
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
            // TODO: Check database for user ID and use userId parameter
            UserData user = null;

            // TODO: Make sure the page has the information needed to display the profile
            ViewBag.User = new UserDisplay 
            {
                Username = "TODO:Username",
                PermissionGroup = (user?.PermissionLevel ?? Permissions.Default).ToString(),
                Avatar = "https://via.placeholder.com/128",
                JoinDate = user?.JoinDate ?? DateTime.Today
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