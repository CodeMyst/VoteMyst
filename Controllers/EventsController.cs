using System;
using System.Linq;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

using VoteMyst.Database;
using VoteMyst.Database.Models;
using VoteMyst.PermissionSystem;

namespace VoteMyst.Controllers
{
    /// <summary>
    /// Provides a controller that handles events.
    /// </summary>
    public class EventsController : VoteMystController
    {
        /// <summary>
        /// Represents the result of a voting action.
        /// </summary>
        public class VoteActionResult
        {
            /// <summary>
            /// Indicates if the action was successful.
            /// </summary>
            public bool ActionSuccess { get; set; }
            /// <summary>
            /// Indicates if the entry has a vote after the action has performed.
            /// </summary>
            public bool HasVote { get; set; }

            public VoteActionResult(bool success, bool hasVote)
            {
                ActionSuccess = success;
                HasVote = hasVote;
            }
        }

        public EventsController(IServiceProvider serviceProvider) : base(serviceProvider) { }

        /// <summary>
        /// Displays an overview of all events.
        /// </summary>
        [RequirePermissions(Permissions.ViewEvents)]
        public IActionResult Index()
        {
            UserData user = GetCurrentUser();
            Event[] finishedEvents = DatabaseHelpers.Events.GetAllEventsFinishedBefore(DateTime.UtcNow);
            Event[] plannedEvents = DatabaseHelpers.Events.GetPlannedEvents(user.HasPermission(Permissions.CreateEvents));
            Event[] ongoingEvents = DatabaseHelpers.Events.GetCurrentEvents();

            ViewBag.History = finishedEvents;
            ViewBag.Planned = plannedEvents;
            ViewBag.Current = ongoingEvents;

            return View();
        }

        /// <summary>
        /// Displays the entry with the specified ID.
        /// </summary>
        [RequirePermissions(Permissions.ViewEntries)]
        public IActionResult Display(int id) 
        {
            Event e = DatabaseHelpers.Events.GetEvent(id);
            ViewBag.Event = e;

            if (e == null)
                return NotFound();
                
            EventState eventState = e.GetCurrentState();

            if (eventState == EventState.Hidden)
            {
                // If the event is not revealed yet, don't allow to find it
                return NotFound();
            }
            if (eventState == EventState.Ongoing)
            {
                // If submissions are still open, display all submissions so far
                ViewBag.Entries = DatabaseHelpers.Entries.GetEntriesInEvent(e);
            }
            if (eventState == EventState.Voting) 
            {
                // If the event voting phase has started and not ended yet, redirect to the vote page
                return RedirectToAction("vote", new { id = id });
            }
            if (eventState == EventState.Closed)
            {
                // If voting has ended, display the winners in order
                ViewBag.Leaderboard = DatabaseHelpers.Entries.GetEntriesInEvent(e)
                    .Select(e => (DatabaseHelpers.Votes.GetAllVotesForEntry(e).Length, e))
                    .OrderByDescending(e => e.Item1)
                    .ThenBy(e => e.Item2.EntryId)
                    .ToArray();
            }

            return View();
        }

        /// <summary>
        /// Provides the page to create a new event.
        /// </summary>
        [RequirePermissions(Permissions.CreateEvents)]
        public IActionResult New() 
        {
            return View(new Event());
        }

        /// <summary>
        /// Creates a new event in the database.
        /// </summary>
        [HttpPost]
        [RequirePermissions(Permissions.CreateEvents)]
        public IActionResult New([FromForm] Event e)
        {
            if (!ModelState.IsValid)
            {
                string[] errorMessages = ModelState.Values.SelectMany(value => value.Errors).Select(error => error.ErrorMessage).ToArray();
                ViewBag.ErrorMessages = errorMessages;

                return View(e);
            }
            else
            {
                DatabaseHelpers.Events.CreateEvent(e);

                return View("Success", e);
            }
        }

        [RequirePermissions(Permissions.ViewEntries)]
        public IActionResult Vote(int id) 
        {
            // TODO: Maybe support multiple events?
            Event currentEvent = DatabaseHelpers.Events.GetCurrentEvents().FirstOrDefault(e => e.EventId == id);

            if (currentEvent == null)
                return NotFound();
            if (currentEvent.GetCurrentState() != EventState.Voting)
                return Unauthorized();

            Entry[] entries = DatabaseHelpers.Entries.GetEntriesInEvent(currentEvent);

            Random rnd = new Random();
            Entry[] randomizedEntries = entries.OrderBy(e => rnd.Next()).ToArray();

            ViewBag.Event = currentEvent;
            ViewBag.RandomizedEntries = randomizedEntries;

            return View();
        }

        [HttpPost]
        [Route("vote/cast/{eventId}/{entryId}")]
        public IActionResult CastVote(int eventId, int entryId)
        {
            // Disallow anonymous voting
            if (!User.Identity.IsAuthenticated)
                return Forbid();

            Entry entry = DatabaseHelpers.Entries.GetEntry(entryId);

            UserData user = GetCurrentUser();
            UserData author = DatabaseHelpers.Users.GetUser(entry.UserId);
            
            // Disallow voting on own posts
            if (user.UserId == author.UserId)
                return Unauthorized();

            Event entryEvent = DatabaseHelpers.Events.GetEvent(eventId);
            
            // Only allow voting while its open
            if (entryEvent.GetCurrentState() != EventState.Voting)
                return Unauthorized();

            Vote vote = DatabaseHelpers.Votes.GetVoteByUserOnEntry(user.UserId, entryId);

            // Make sure a vote by the user on the specified entry does not exist yet
            if (vote != null)
                return Ok(new VoteActionResult(false, true));
            
            // If all checks passed, cast the vote
            DatabaseHelpers.Votes.AddVote(entryId, user.UserId);

            return Ok(new VoteActionResult(true, true));
        }
        
        [HttpPost]
        [Route("vote/remove/{eventId}/{entryId}")]
        public IActionResult RemoveVote(int eventId, int entryId)
        {
            // Disallow anonymous voting
            if (!User.Identity.IsAuthenticated)
                return Forbid();
                
            Event entryEvent = DatabaseHelpers.Events.GetEvent(eventId);

            // Only allow deleting votes if voting is still open
             if (entryEvent.GetCurrentState() != EventState.Voting)
                return Unauthorized();

            UserData user = GetCurrentUser();
            Vote vote = DatabaseHelpers.Votes.GetVoteByUserOnEntry(user.UserId, entryId);

            // Make sure a vote exists that can be deleted
            if (vote == null)
                return Ok(new VoteActionResult(false, false));

            DatabaseHelpers.Votes.DeleteVote(entryId, user.UserId);
            return Ok(new VoteActionResult(true, false));
        }
    }
}