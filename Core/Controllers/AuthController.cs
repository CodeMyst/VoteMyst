using System;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;

using VoteMyst.Database;

namespace VoteMyst.Controllers
{
    /// <summary>
    /// Provides the homepage for the site.
    /// </summary>
    public class AuthController : VoteMystController
    {
        public AuthController(IServiceProvider serviceProvider) : base(serviceProvider) { }

        [Route("login")]
        public IActionResult Login()
        {
            return View("Login");
        }

        [Route("user/me/auth")]
        public IActionResult ListAuthentications()
        {
            if (!User.Identity.IsAuthenticated)
                return Unauthorized();

            return View("List");
        }

        /// <summary>
        /// Provides the endpoint to log in via authorization methods. Redirects to "/users/me" afterwards.
        /// </summary>
        [Route("login/{service}")]
        public IActionResult Login(Service service)
        {
            return Challenge(new AuthenticationProperties { RedirectUri = "/users/me" }, service.ToString().ToLower());
        }

        /// <summary>
        /// Provides the endpoint to log out from the current session. This also clears the session cookies.
        /// </summary>
        [Route("logout")]
        public IActionResult Logout()
        {
            Response.Cookies.Delete("AuthCookie");
            return Redirect("/");
        }
    }
}