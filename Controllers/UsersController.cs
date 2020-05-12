using System;
using System.Linq;
using System.Text.RegularExpressions;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;

using VoteMyst.Database;
using VoteMyst.Database.Models;
using VoteMyst.PermissionSystem;

namespace VoteMyst.Controllers
{
    /// <summary>
    /// Provides a controller to handle users (including self).
    /// </summary>
    public class UsersController : VoteMystController
    {
        private static readonly Permissions[] _permissions = (Permissions[])Enum.GetValues(typeof(Permissions));
        private static readonly AccountState[] _groups =  AccountStateExtension.ApplyableStates;

        public UsersController(IServiceProvider serviceProvider) : base(serviceProvider) { }

        /// <summary>
        /// Wipes the data of the current user and subsequentially logs him out.
        /// </summary>
        public IActionResult WipeAccount() 
        {
            UserData selfUser = GetCurrentUser();
            DatabaseHelpers.Users.WipeUser(selfUser.UserId);
            return Logout();
        }

        /// <summary>
        /// Provides the page to search for users.
        /// </summary>
        public IActionResult Search() 
        {
            throw new NotImplementedException();
        }
        
        /// <summary>
        /// Provides the endpoint to log in via authorization methods. Redirects to "/users/me" afterwards.
        /// </summary>
        [Route("login")]
        public IActionResult Login()
        {
            return Challenge(new AuthenticationProperties { RedirectUri = "/users/me" });
        }

        /// <summary>
        /// Provides the endpoint to log out from the current session. This also clears the session cookies.
        /// </summary>
        [Route("logout")]
        public IActionResult Logout()
        {
            Response.Cookies.Delete("DiscordCookie");
            return RedirectToAction("Index", "Home");
        }

        /// <summary>
        /// Displays the user with the specified ID.
        /// </summary>
        [RequirePermissions(Permissions.ViewUsers)]
        public IActionResult Display(string id) 
        {
            UserData targetUser = ProfileBuilder.FromId(id);
            if (targetUser == null)
                return NotFound();

            UserData selfUser = ProfileBuilder.FromPrincipal(User);
            Entry[] entries = DatabaseHelpers.Entries.GetEntriesFromUser(targetUser);

            ViewBag.IsSelf = targetUser.DisplayId == selfUser.DisplayId;

            ViewBag.SelfUser = selfUser;
            ViewBag.InspectedUser = targetUser;

            ViewBag.InspectedUserGroup = targetUser.GetAccountState();
            ViewBag.InspectedUserGroupLevel = (int) targetUser.AccountState;
            ViewBag.InspectedTotalEntries = entries.Length;
            ViewBag.InspectedTotalVotes = entries.Select(e => DatabaseHelpers.Votes.GetAllVotesForEntry(e).Length).Sum();
            ViewBag.AllowAdminDashboard = selfUser.HasPermission(Permissions.ModifyUsers);

            if (ViewBag.AllowAdminDashboard)
            {                
                ViewBag.UserPermissions = (ulong)targetUser.PermissionLevel;
                ViewBag.PermissionEntries = _permissions
                    .Select(p => (string.Join(' ', (Regex.Split(p.ToString(), "(?=[A-Z][^A-Z])"))), (ulong)p, targetUser.PermissionLevel.HasFlag(p)))
                    .ToArray();
                ViewBag.AccountGroups = _groups
                    .Select(g => (g.ToString(), (ulong)g))
                    .ToArray();
            }

            return View("Display");
        }

        /// <summary>
        /// Displays the user page for the currently authenticated user.
        /// </summary>
        public IActionResult Me()
        {
            if (!User.Identity.IsAuthenticated)
                return Forbid();

            UserData selfUser = ProfileBuilder.FromPrincipal(User);

            if (selfUser.IsBanned())
                return Forbid();

            return Display(selfUser.DisplayId);
        }

        /// <summary>
        /// Provides the endpoint to edit a user.
        /// </summary>
        [HttpPost]
        [RequirePermissions(Permissions.ModifyUsers)]
        public IActionResult Edit( 
            string id,
            string username = null,
            string avatar = null,
            ulong? permissions = 0,
            int? group = 0)
        {
            UserData user = DatabaseHelpers.Users.GetUser(id);
            
            if (permissions.HasValue && (ulong) user.PermissionLevel != permissions) 
                DatabaseHelpers.Users.SetPermission(DatabaseHelpers.Users.GetUser(id), (Permissions) permissions.Value);

            if (group.HasValue && (int) user.AccountState != group) {
                DatabaseHelpers.Users.SetPermission(user, ((AccountState) group).GetDefaultPermissions());
                DatabaseHelpers.Users.SetAccountState(user, (AccountState) group.Value);
            }

            return RedirectToAction("display", new { id = id });
        }
    }
}