using System;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;

using VoteMyst.Database;
using VoteMyst.Controllers.Api.Models;
using Microsoft.AspNetCore.Http;

namespace VoteMyst.Controllers.Api
{
    public class EntryApiController : VoteMystApiController
    {
        private ILogger _logger;

        public EntryApiController(IServiceProvider serviceProvider) : base(serviceProvider)
        {
            _logger = serviceProvider.GetService<ILogger<EntryApiController>>();
        }

        [HttpPost]
        [Route("api/entry/{id}/delete")]
        public IActionResult Delete(string id)
        {
            throw new NotImplementedException();
        }

        [HttpPost]
        [Route("api/entry/{id}/votes/cast")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public ActionResult<VoteResult> CastVote(string id)
        {
            if (!User.Identity.IsAuthenticated)
                return Unauthorized("Anonymous voting is not allowed.");

            Entry entry = DatabaseHelpers.Context.QueryByDisplayID<Entry>(id);
            Event entryEvent = entry.Event;

            UserAccount user = GetCurrentUser();
            UserAccount author = entry.Author;

            if (user.ID == author.ID)
                return BadRequest("Voting on own posts is forbidden.");
            if (entryEvent.GetCurrentState() != EventState.Voting)
                return BadRequest("Voting is currently not available for this event.");

            Vote vote = DatabaseHelpers.Votes.GetVoteByUserOnEntry(user, entry);

            // Make sure a vote by the user on the specified entry does not exist yet
            if (vote != null)
            {
                return Ok(new VoteResult(false, true));
            }
            else
            {
                // If all checks passed, cast the vote
                DatabaseHelpers.Votes.AddVote(entry, user);

                _logger.LogInformation("{0} cast a vote on {1}.", user, entry);

                return Ok(new VoteResult(true, true));
            }

            
        }

        [HttpPost]
        [Route("api/entry/{id}/votes/remove")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public ActionResult<VoteResult> RemoveVote(string id)
        {
            if (!User.Identity.IsAuthenticated)
                return Unauthorized();

            UserAccount user = GetCurrentUser();

            Entry entry = DatabaseHelpers.Context.QueryByDisplayID<Entry>(id);
            Event entryEvent = entry.Event;

            if (entryEvent.GetCurrentState() != EventState.Voting)
                return BadRequest("Voting is currently not available for this event.");

            Vote vote = DatabaseHelpers.Votes.GetVoteByUserOnEntry(user, entry);

            // Make sure a vote exists that can be deleted
            if (vote != null)
            {
                // If all checks passed, delete the vote
                _logger.LogInformation("{0} removed their vote on {1}.", user, entry);

                DatabaseHelpers.Votes.DeleteVote(vote);

                return Ok(new VoteResult(true, false));
            }
            else
            {
                return Ok(new VoteResult(false, false));
            }
        }
    }
}
