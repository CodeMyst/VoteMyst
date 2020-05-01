using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

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

        public IActionResult Search() 
        {
            return View();
        }
        public IActionResult Display(int userId) 
        {
            // JUST FOR TESTING
            if (userId % 2 == 0)
            {
                return View("NotFound");
            }

            // Make sure the page has the information needed to display the profile
            ViewBag.User = new UserDisplay 
            {
                DiscordName = "Yilian",
                DiscordTag = "2345",
                PermissionGroup = "Admin",
                AvatarUrl = "/examples/ex01.png",
                JoinDate = DateTime.Now
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