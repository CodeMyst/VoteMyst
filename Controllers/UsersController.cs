using System;
using System.Linq;
using System.Collections.Generic;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;

using VoteMyst.Database;
using VoteMyst.Authorization;

using SixLabors.ImageSharp;

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
        private readonly AvatarHelper _avatarHelper;

        public UsersController(ILogger<UsersController> logger, IServiceProvider serviceProvider) : base(serviceProvider)
        {
            _logger = logger;
            _avatarHelper = serviceProvider.GetService<AvatarHelper>();
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
        /// Displays the user with the specified ID.
        /// </summary>
        [Route("users/{id}")]
        public IActionResult Display(string id) 
        {
            if (!User.Identity.IsAuthenticated)
                return View("Forbidden");

            UserAccount targetUser = ProfileBuilder.FromId(id);
            UserAccount selfUser = GetCurrentUser();

            if (targetUser == null)
                return NotFound();
            if (targetUser.ID == selfUser.ID)
                return RedirectToAction("me");

            return View("Display", targetUser);
        }

        /// <summary>
        /// Displays the user page for the currently authenticated user.
        /// </summary>
        [Route("users/me")]
        public IActionResult Me()
        {
            if (!User.Identity.IsAuthenticated)
                return View("Forbidden");

            UserAccount selfUser = GetCurrentUser();

            return View("Display", selfUser);
        }

        /// <summary>
        /// Displays the editing page for the specified user.
        /// </summary>
        [Route("users/{id}/edit")]
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
        /// Displays the personalized data of a user.
        /// </summary>
        [Route("users/{id}/data")]
        public IActionResult Data(string id)
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
        [Route("users/{id}/edit")]
        public IActionResult Edit(string id, [FromForm, Bind] UserAccount accountChanges, [FromForm] IFormFile avatar, [FromForm] bool clearAvatar)
        {
            UserAccount targetUser = ProfileBuilder.FromId(id);
            UserAccount selfUser = GetCurrentUser();

            bool isSelf = targetUser.ID == selfUser.ID;
            bool canManageSelf = selfUser.Permissions.HasFlag(GlobalPermissions.ManageSelf);
            bool canManageUsers = selfUser.Permissions.HasFlag(GlobalPermissions.ManageUsers);

            if (!(isSelf && canManageSelf || canManageUsers))
                return Forbid();

            // Collect the initial model errors
            List<string> errorMessages = new List<string>();
            if (!ModelState.IsValid)
            {
                errorMessages = ModelState.Values.SelectMany(value => value.Errors).Select(error => error.ErrorMessage).ToList();
            }

            // Perform additional validation
            if (avatar != null)
            {
                if (avatar.ContentType != "image/png")
                    errorMessages.Add("You can only use PNG images as avatars.");
                if (avatar.Length > 4194304)
                    errorMessages.Add("Avatar images can only be 4MB in size.");
                
                using (var image = Image.Load(avatar.OpenReadStream()))
                {
                    if (image.Width != image.Height)
                        errorMessages.Add("Avatar images must be in a square (1:1) aspect ratio.");
                }
            }

            if (errorMessages.Count > 0)
            {
                // If validation errors occured, display them on the edit page.
                ViewBag.ErrorMessages = errorMessages.ToArray();
                return Edit(id);
            }

            targetUser.Username = accountChanges.Username;

            string userAvatarPath = System.IO.Path.Combine(Environment.WebRootPath, _avatarHelper.GetAbsoluteAvatarUrl(targetUser));
            if (avatar != null) 
            {
                using (var localFile = System.IO.File.OpenWrite(userAvatarPath))
                using (var uploadedFile = avatar.OpenReadStream())
                {
                    uploadedFile.CopyTo(localFile);
                }
            }
            else
            {
                if (clearAvatar)
                {
                    System.IO.File.Delete(userAvatarPath);
                }
            }

            if (canManageUsers)
            {
                if (accountChanges.Permissions != targetUser.Permissions)
                {
                    targetUser.Permissions = accountChanges.Permissions;
                }
                if (accountChanges.AccountBadge != targetUser.AccountBadge)
                {
                    targetUser.AccountBadge = accountChanges.AccountBadge;
                }
            }

            DatabaseHelpers.Context.UpdateAndSave(targetUser);

            return targetUser.ID == selfUser.ID
                ? RedirectToAction("me")
                : RedirectToAction("display", new { id });
        }
        
        [HttpPost]
        [Route("users/{id}/refreshdatafromservice")]
        public IActionResult RefreshDataFromService(string id)
        {
            if (!User.Identity.IsAuthenticated)
                return Unauthorized();

            UserAccount targetUser = ProfileBuilder.FromId(id);
            UserAccount selfUser = GetCurrentUser();

            if (targetUser != selfUser)
                return Unauthorized();

            ProfileBuilder.UpdateDataFromService(User);

            _logger.LogInformation("{0} refreshed their data from Discord.", selfUser);
            
            return RedirectToAction("me");
        }
    }
}