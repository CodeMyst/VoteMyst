using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

using VoteMyst.Database;
using VoteMyst.Database.Models;
using VoteMyst.PermissionSystem;

namespace VoteMyst.Controllers
{
    public class VoteController : Controller
    {
        public class VoteActionData 
        {
            public int EntryId { get; set; }
        }
        public class VoteActionResult
        {
            public bool ActionSuccess { get; set; }
            public bool HasVote { get; set; }

            public VoteActionResult(bool success, bool hasVote)
            {
                ActionSuccess = success;
                HasVote = hasVote;
            }
        }

        private readonly UserProfileBuilder _builder;
        private readonly DatabaseHelperProvider _helpers;

        public VoteController(UserProfileBuilder builder, DatabaseHelperProvider helpers)
        {
            _builder = builder;
            _helpers = helpers;
        }

        [RequirePermissions(Permissions.ViewEntries)]
        public IActionResult Index() 
        {
            Event[] currentEvents = _helpers.Events.GetCurrentEvents();
            if (currentEvents.Length == 0)
                return NotFound();

            // TODO: Maybe support multiple events?

            Event currentEvent = currentEvents[0];
            Entry[] entries = _helpers.Entries.GetEntriesInEvent(currentEvent);

            Random rnd = new Random();
            Entry[] randomizedEntries = entries.OrderBy(e => rnd.Next()).ToArray();

            ViewBag.Entries = entries;
            ViewBag.RandomizedEntries = randomizedEntries;

            return View();
        }

        [HttpPost]
        public IActionResult Cast([FromBody] VoteActionData data)
        {
            if (!User.Identity.IsAuthenticated)
                return Unauthorized();

            UserData user = _builder.FromPrincipal(User);
            Vote vote = _helpers.Votes.GetVoteByUserOnEntry(user.UserId, data.EntryId);

            if (vote != null)
                return Ok(new VoteActionResult(false, true));
            
            _helpers.Votes.AddVote(data.EntryId, user.UserId);
            return Ok(new VoteActionResult(true, true));
        }
        
        [HttpPost]
        public IActionResult Remove([FromBody] VoteActionData data)
        {
            if (!User.Identity.IsAuthenticated)
                return Unauthorized();

            UserData user = _builder.FromPrincipal(User);
            Vote vote = _helpers.Votes.GetVoteByUserOnEntry(user.UserId, data.EntryId);

            if (vote == null)
                return Ok(new VoteActionResult(false, false));

            _helpers.Votes.DeleteVote(data.EntryId, user.UserId);
            return Ok(new VoteActionResult(true, false));
        }
    }
}