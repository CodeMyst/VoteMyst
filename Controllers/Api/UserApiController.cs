using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using VoteMyst.Database;

namespace VoteMyst.Controllers.Api
{
    public class UserApiController : VoteMystApiController
    {
        public UserApiController(IServiceProvider serviceProvider) : base(serviceProvider) { }

        [HttpPost]
        [Route("api/users/delete")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public IActionResult DeleteOwnAccount()
        {
            if (!User.Identity.IsAuthenticated)
                return Unauthorized();

            UserAccount selfUser = GetCurrentUser();

            DatabaseHelpers.Users.WipeUser(selfUser);

            return Ok();
        }
    }
}
