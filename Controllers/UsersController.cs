using System;
using System.Linq;
using System.Text.RegularExpressions;

using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.AspNetCore.Authentication;

using VoteMyst.Database;
using VoteMyst.PermissionSystem;

namespace VoteMyst.Controllers
{
    /// <summary>
    /// Provides a controller to handle users (including self).
    /// </summary>
    public class UsersController : VoteMystController
    {
        private static readonly GlobalPermissions[] _permissions = Enum.GetValues(typeof(GlobalPermissions))
            .Cast<GlobalPermissions>().Where(x => (x != 0) && ((x & (x - 1)) == 0)).ToArray();
        private static readonly AccountBadge[] _badges = (AccountBadge[])Enum.GetValues(typeof(AccountBadge));

        private readonly ILogger _logger;

        public UsersController(ILogger<UsersController> logger, IServiceProvider serviceProvider) : base(serviceProvider)
        {
            _logger = logger;
        }

        /// <summary>
        /// Wipes the data of the current user and subsequentially logs him out.
        /// </summary>
        public IActionResult WipeAccount() 
        {
            UserAccount selfUser = GetCurrentUser();
            DatabaseHelpers.Users.WipeUser(selfUser);

            _logger.LogWarning("{0} wiped their account.", selfUser);

            return Logout();
        }

        /// <summary>
        /// Provides the page to search for users.
        /// </summary>
        [RequireGlobalPermission(GlobalPermissions.ManageUsers)]
        public IActionResult Search() 
        {
            string queryName = Request.Query["name"];
            UserAccount[] queryResult = null;

            if (!string.IsNullOrEmpty(queryName))
            {
                queryResult = DatabaseHelpers.Users.SearchUsers(queryName);
            }

            return View(queryResult);
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
        public IActionResult Display(string id) 
        {
            UserAccount targetUser = ProfileBuilder.FromId(id);
            if (targetUser == null)
                return NotFound();

            return View("Display", targetUser);
        }

        /// <summary>
        /// Displays the user page for the currently authenticated user.
        /// </summary>
        public IActionResult Me()
        {
            if (!User.Identity.IsAuthenticated)
                return Forbid();

            UserAccount selfUser = ProfileBuilder.FromPrincipal(User);

            return Display(selfUser.DisplayID);
        }

        public IActionResult Edit(string id)
        {
            UserAccount selfUser = ProfileBuilder.FromPrincipal(User);
            UserAccount inspectedUser = ProfileBuilder.FromId(id);

            if (inspectedUser == null)
                return NotFound();
            
            if (!selfUser.Permissions.HasFlag(GlobalPermissions.ManageUsers) && selfUser.ID != inspectedUser.ID)
                return Forbid();

            return View(inspectedUser);
        }

        /// <summary>
        /// Provides the endpoint to edit a user.
        /// </summary>
        [HttpPost]
        public IActionResult Edit(string id, [FromForm, Bind] UserAccount accountChanges, [FromForm] IFormFile avatar)
        {
            UserAccount targetUser = ProfileBuilder.FromId(id);
            UserAccount selfUser = ProfileBuilder.FromPrincipal(User);

            bool canManageUsers = selfUser.Permissions.HasFlag(GlobalPermissions.ManageUsers);

            if (targetUser.ID != selfUser.ID && !canManageUsers)
                return Forbid();

            targetUser.Username = accountChanges.Username;
            
            if (avatar != null) 
            {
                // TODO: Implement avatar changing
            }

            if (accountChanges.Permissions != targetUser.Permissions || accountChanges.AccountBadge != targetUser.AccountBadge)
            {
                if (!canManageUsers)
                    return Forbid();

                targetUser.Permissions = accountChanges.Permissions;
                targetUser.AccountBadge = accountChanges.AccountBadge;
            }

            DatabaseHelpers.Users.SaveUser(targetUser);

            return targetUser.ID == GetCurrentUser().ID
                ? RedirectToAction("me")
                : RedirectToAction("display", new { id = id });
        }
    }
}