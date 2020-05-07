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
            public int EventId { get; set;}
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
            // TODO: Maybe support multiple events?
            Event currentEvent = _helpers.Events.GetCurrentEvents().FirstOrDefault();

            if (currentEvent == null)
                return NotFound();

            if (DateTime.UtcNow < currentEvent.EndDate || DateTime.UtcNow > currentEvent.VoteEndDate)
                return NotFound();

            Entry[] entries = _helpers.Entries.GetEntriesInEvent(currentEvent);

            Random rnd = new Random();
            Entry[] randomizedEntries = entries.OrderBy(e => rnd.Next()).ToArray();

            ViewBag.Event = currentEvent;
            ViewBag.RandomizedEntries = randomizedEntries;

            return View();
        }

        [HttpPost]
        public IActionResult Cast([FromBody] VoteActionData data)
        {
            // Disallow anonymous voting
            if (!User.Identity.IsAuthenticated)
                return Unauthorized();

            Entry entry = _helpers.Entries.GetEntry(data.EntryId);

            UserData user = _builder.FromPrincipal(User);
            UserData author = _helpers.Users.GetUser(entry.UserId);
            
            // Disallow voting on own posts
            if (user.UserId == author.UserId)
                return Unauthorized();

            Event entryEvent = _helpers.Events.GetEvent(data.EventId);
            
            // Only allow voting while its open
            if (DateTime.UtcNow < entryEvent.EndDate || DateTime.UtcNow > entryEvent.VoteEndDate)
                return Unauthorized();

            Vote vote = _helpers.Votes.GetVoteByUserOnEntry(user.UserId, data.EntryId);

            // Make sure a vote by the user on the specified entry does not exist yet
            if (vote != null)
                return Ok(new VoteActionResult(false, true));
            
            // If all checks passed, cast the vote
            _helpers.Votes.AddVote(data.EntryId, user.UserId);

            return Ok(new VoteActionResult(true, true));
        }
        
        [HttpPost]
        public IActionResult Remove([FromBody] VoteActionData data)
        {
            // Disallow anonymous voting
            if (!User.Identity.IsAuthenticated)
                return Unauthorized();
                
            Event entryEvent = _helpers.Events.GetEvent(data.EventId);

            // Only allow deleting votes if voting is still open
             if (DateTime.UtcNow < entryEvent.EndDate || DateTime.UtcNow > entryEvent.VoteEndDate)
                return Unauthorized();

            UserData user = _builder.FromPrincipal(User);
            Vote vote = _helpers.Votes.GetVoteByUserOnEntry(user.UserId, data.EntryId);

            // Make sure a vote exists that can be deleted
            if (vote == null)
                return Ok(new VoteActionResult(false, false));

            _helpers.Votes.DeleteVote(data.EntryId, user.UserId);
            return Ok(new VoteActionResult(true, false));
        }
    }
}