using System;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

using VoteMyst.Discord;
using VoteMyst.Database;
using VoteMyst.Database.Models;
using VoteMyst.PermissionSystem;

namespace VoteMyst.Controllers
{
    public class UserController : Controller
    {
        private readonly UserProfileBuilder _profileBuilder;
        private readonly DatabaseHelperProvider _helpers;

        public UserController(UserProfileBuilder profileBuilder, DatabaseHelperProvider helpers) 
        {
            _profileBuilder = profileBuilder;
            _helpers = helpers;
        }

        [RequirePermissions(Permissions.ViewUsers)]
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

        [RequirePermissions(Permissions.ViewUsers)]
        public IActionResult Display(string displayId) 
        {
            UserData targetUser = _profileBuilder.FromId(displayId);
            if (targetUser == null)
                return View("NotFound");

            UserData selfUser = _profileBuilder.FromPrincipal(User);
            Entry[] entries = _helpers.Entries.GetEntriesFromUser(selfUser);

            ViewBag.IsSelf = targetUser.DisplayId == selfUser.DisplayId;

            ViewBag.SelfUser = selfUser;
            ViewBag.InspectedUser = targetUser;

            ViewBag.InspectedUserGroup = targetUser.DeterminePermissionGroup();
            ViewBag.InspectedTotalEntries = entries.Length;
            ViewBag.InspectedTotalVotes = entries.Select(e => _helpers.Votes.GetAllVotesForEntry(e).Length).Sum();

            bool allowAdminDashboard = targetUser.DisplayId != selfUser.DisplayId
                && selfUser.IsAdmin() && !targetUser.IsAdmin();
            ViewBag.AllowAdminDashboard = allowAdminDashboard;
            if (allowAdminDashboard)
            {
                Permissions[] permissions = Enum.GetValues(typeof(Permissions)).Cast<Permissions>().ToArray();
                Permissions[] groups = new Permissions[] {
                    Permissions.Banned, Permissions.Guest, Permissions.Default, Permissions.Moderator, Permissions.Admin
                };
                Permissions[] permissionsNoGroups = permissions.Except(groups).ToArray();
                
                ViewBag.UserPermissions = (ulong)targetUser.PermissionLevel;
                ViewBag.PermissionEntries = permissionsNoGroups
                    .Select(p => (string.Join(' ', (Regex.Split(p.ToString(), "(?=[A-Z][^A-Z])"))), (ulong)p, targetUser.PermissionLevel.HasFlag(p)))
                    .ToArray();
                ViewBag.PermissionGroups = groups
                    .Select(g => (g.ToString(), (ulong)g))
                    .ToArray();
            }

            return View("Display");
        }

        public IActionResult DisplaySelf()
        {
            if (!User.Identity.IsAuthenticated)
                return Forbid();

            UserData selfUser = _profileBuilder.FromPrincipal(User);
            return Display(selfUser.DisplayId);
        }

        [RequirePermissions(Permissions.ModifyUsers)]
        public IActionResult BanUser(int id)
        {
            throw new NotImplementedException();
        }

        [HttpPost]
        [RequirePermissions(Permissions.ModifyUsers)]
        public IActionResult Display(string displayId, 
            string username = null,
            string avatar = null,
            ulong? permissions = 0)
        {
            if (permissions.HasValue)
                _helpers.Users.SetPermission(_helpers.Users.GetUser(displayId), (Permissions)permissions.Value);

            return Display(displayId);
        }
    }
}