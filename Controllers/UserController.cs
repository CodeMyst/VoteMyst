using System;
using System.Linq;
using System.Dynamic;
using System.Collections.Generic;
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
        private static Permissions[] _permissions = Enum.GetValues(typeof(Permissions)).Cast<Permissions>().ToArray();
        private static AccountState[] _groups =  AccountStateExtension.ApplyableStates;

        private readonly UserProfileBuilder _profileBuilder;
        private readonly DatabaseHelperProvider _helpers;
        private readonly TestDataCreator _testData;

        public UserController(UserProfileBuilder profileBuilder, DatabaseHelperProvider helpers, TestDataCreator testData) 
        {
            _profileBuilder = profileBuilder;
            _helpers = helpers;
            _testData = testData;
        }

        public IActionResult CreateTestData()
        {
            _testData.CreateUserData();

            return DisplaySelf();
        }

        public IActionResult WipeAccount() 
        {
            UserData selfUser = _profileBuilder.FromPrincipal(User);
            _helpers.Users.WipeUser(selfUser.UserId);
            return Logout();
        }

        [RequirePermissions(Permissions.ModifyUsers)]
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
            Entry[] entries = _helpers.Entries.GetEntriesFromUser(targetUser);

            ViewBag.IsSelf = targetUser.DisplayId == selfUser.DisplayId;

            ViewBag.SelfUser = selfUser;
            ViewBag.InspectedUser = targetUser;

            ViewBag.InspectedUserGroup = targetUser.GetAccountState();
            ViewBag.InspectedUserGroupLevel = (int) targetUser.AccountState;
            ViewBag.InspectedTotalEntries = entries.Length;
            ViewBag.InspectedTotalVotes = entries.Select(e => _helpers.Votes.GetAllVotesForEntry(e).Length).Sum();
            ViewBag.AllowAdminDashboard = selfUser.HasPermission(Permissions.ModifyUsers);

            if (ViewBag.AllowAdminDashboard)
            {                
                ViewBag.UserPermissions = (ulong)targetUser.PermissionLevel;
                ViewBag.PermissionEntries = _permissions
                    .Select(p => (p.ToString(), (ulong)p, targetUser.PermissionLevel.HasFlag(p)))
                    .ToArray();
                ViewBag.AccountGroups = _groups
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

            if (selfUser.IsBanned())
                return Forbid();

            return Display(selfUser.DisplayId);
        }

        [HttpPost]
        [RequirePermissions(Permissions.ModifyUsers)]
        public IActionResult Display( 
            string userId,
            string username = null,
            string avatar = null,
            ulong? permissions = 0,
            int? group = 0)
        {
            UserData user = _helpers.Users.GetUser(userId);
            
            if (permissions.HasValue && (ulong) user.PermissionLevel != permissions) 
                _helpers.Users.SetPermission(_helpers.Users.GetUser(userId), (Permissions) permissions.Value);

            if (group.HasValue && (int) user.AccountState != group) {
                _helpers.Users.SetPermission(user, ((AccountState) group).GetDefaultPermissions());
                _helpers.Users.SetAccountState(user, (AccountState) group.Value);
            }

            return Display(userId);
        }
    }
}