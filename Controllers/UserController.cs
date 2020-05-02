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
        private UserProfileBuilder _profileBuilder;
        private EntryHelper _entryHelper;
        private VoteHelper _voteHelper;

        public UserController(UserProfileBuilder profileBuilder, EntryHelper entryHelper, VoteHelper voteHelper) 
        {
            _profileBuilder = profileBuilder;
            _entryHelper = entryHelper;
            _voteHelper = voteHelper;
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
            Entry[] entries = _entryHelper.GetEntriesFromUser(selfUser);

            // TODO: Make sure the page has the information needed to display the profile
            ViewBag.IsSelf = user.DisplayId == selfUser.DisplayId;
            ViewBag.Username = user.Username;
            ViewBag.DisplayId = user.DisplayId;
            ViewBag.PermissionsGroup = user.PermissionLevel.ToString();
            ViewBag.JoinDate = user.JoinDate;
            ViewBag.TotalEntries = entries.Length;
            ViewBag.TotalVotes = entries.Select(e => _voteHelper.GetAllVotesForEntry(e).Length).Sum();

            return View("Display");
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