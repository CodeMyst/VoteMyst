using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using VoteMyst.Database;

namespace VoteMyst.Controllers.Api
{
    public class EventApiController : VoteMystApiController
    {
        public EventApiController(IServiceProvider serviceProvider) : base(serviceProvider) { }

        [HttpPost]
        [Route("api/events/{eventId}/hosts/add/{userId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public IActionResult AddHost(string eventId, string userId)
        {
            UserAccount user = GetCurrentUser();
            Event targetEvent = DatabaseHelpers.Events.GetEventByUrl(eventId);

            if (targetEvent == null)
                return BadRequest("The specified event does not exist.");

            EventPermissions permissions = DatabaseHelpers.Events.GetUserPermissionsForEvent(user, targetEvent);
            if (!permissions.HasFlag(EventPermissions.EditEventSettings))
                return Unauthorized();

            UserAccount targetUser = DatabaseHelpers.Context.QueryByDisplayID<UserAccount>(userId);
            if (targetUser == null)
                return BadRequest("The specified user does not exist.");
            if (user.ID == targetUser.ID)
                return BadRequest("Cannot set self as host.");

            DatabaseHelpers.Events.RegisterUserAsHost(targetUser, targetEvent);

            return Ok();
        }

        [HttpPost]
        [Route("api/events/{eventId}/hosts/remove/{userId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public IActionResult RemoveHost(string eventId, [FromBody] string userId)
        {
            UserAccount user = GetCurrentUser();
            Event targetEvent = DatabaseHelpers.Events.GetEventByUrl(eventId);

            if (targetEvent == null)
                return BadRequest("The specified event does not exist.");

            EventPermissions permissions = DatabaseHelpers.Events.GetUserPermissionsForEvent(user, targetEvent);
            if (!permissions.HasFlag(EventPermissions.EditEventSettings))
                return Unauthorized();

            UserAccount targetUser = DatabaseHelpers.Context.QueryByDisplayID<UserAccount>(userId);
            if (targetUser == null)
                return BadRequest("The specified user does not exist.");
            if (user.ID == targetUser.ID)
                return BadRequest("Cannot remove self from hosts.");

            DatabaseHelpers.Events.RemoveUserAsHost(targetUser, targetEvent);

            return Ok();
        }
    }
}
