using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

using Microsoft.Extensions.Logging;
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
        public class Leaderboard : IEnumerable<Leaderboard.Place> 
        {
            public struct Place 
            {
                public int Number { get; set; }
                public UserData Author { get; set; }
                public Entry Entry { get; set; }
                public int Votes { get; set; }

                public static Place FromEntry(Entry entry, DatabaseHelperProvider helpers)
                {
                    return new Place
                    {
                        Number = -1,
                        Author = helpers.Users.GetUser(entry.UserId),
                        Entry = entry,
                        Votes = helpers.Votes.GetAllVotesForEntry(entry).Length
                    };
                }
            }

            private readonly Place[] _places;

            private Leaderboard(Place[] places)
            {
                _places = places;
            }

            public static Leaderboard FromEvent(Event ev, DatabaseHelperProvider helpers)
            {
                Place[] places = helpers.Entries.GetEntriesInEvent(ev)
                    .Select(entry => Place.FromEntry(entry, helpers))
                    .OrderByDescending(place => place.Votes)
                    .ThenBy(place => place.Entry.EntryId)
                    .ToArray();

                int currentPlace = 0;
                int currentVotes = -1;

                for(int i = 0; i < places.Length; i++)
                {
                    if (places[i].Votes != currentVotes)
                        currentPlace++;

                    places[i].Number = currentPlace;
                }

                return new Leaderboard(places);
            }

            public IEnumerator<Place> GetEnumerator()
            {
                foreach (Place p in _places)
                    yield return p;
            }
            IEnumerator IEnumerable.GetEnumerator()
                => _places.GetEnumerator();
        }

        private readonly ILogger _logger;

        public EventsController(ILogger<EventsController> logger, IServiceProvider serviceProvider) : base(serviceProvider)
        {
            _logger = logger;
        }

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

            ViewBag.CanCreateEvents = user.HasPermission(Permissions.CreateEvents);
            ViewBag.History = finishedEvents;
            ViewBag.Planned = plannedEvents;
            ViewBag.Current = ongoingEvents;

            return View();
        }

        /// <summary>
        /// Displays the entry with the specified ID.
        /// </summary>
        [RequirePermissions(Permissions.ViewEntries)]
        public IActionResult Display(string id) 
        {
            UserData user = GetCurrentUser();
            Event e = DatabaseHelpers.Events.GetEvent(id);
            ViewBag.Event = e;

            if (e == null)
                return NotFound();
                
            EventState eventState = e.GetCurrentState();

            // If the event is not revealed yet, don't allow to find it, except if the user is an admin
            if (eventState == EventState.Hidden && user.AccountState != AccountState.Admin)
            {
                _logger.LogWarning("User {0} attempted to access the event with ID {1}, but it is hidden. Sending a 404 response.", user.Username, id);
                return NotFound();
            }
            // If the event voting phase has started and not ended yet, redirect to the vote page
            if (eventState == EventState.Voting) 
            {
                _logger.LogInformation("User {0} attempted to access the event with ID {1}, but voting is in progress. Redirecting to the vote page.", 
                    user.Username, id);
                return RedirectToAction("vote", new { id = id });
            }
            
            return View(e);
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
            if (string.IsNullOrEmpty(e.Url))
            {
                e.Url = Regex.Replace(Regex.Replace(e.Title.ToLowerInvariant().Replace(" ", "-"), 
                    @"[^a-zA-Z\d\-]", string.Empty).Trim('-'), @"-{2,}", "-");
            }

            ModelState.Clear();
            TryValidateModel(e);

            if (!ModelState.IsValid)
            {
                ViewBag.ErrorMessages = ModelState.Values.SelectMany(value => value.Errors).Select(error => error.ErrorMessage).ToArray();

                _logger.LogWarning("User {0} attempted to create an event, but failed with {1} validation errors.",
                    GetCurrentUser().Username, ModelState.ErrorCount);

                return View(e);
            }
            else
            {
                DatabaseHelpers.Events.CreateEvent(e);

                _logger.LogInformation("User {0} created the event '{1}'.", GetCurrentUser().Username, e.Title);

                return View("Success", e);
            }
        }

        [RequirePermissions(Permissions.ViewEntries)]
        public IActionResult Vote(string id) 
        {
            // TODO: Maybe support multiple events?
            Event currentEvent = DatabaseHelpers.Events.GetEvent(id);

            if (currentEvent == null)
                return NotFound();
            if (currentEvent.GetCurrentState() != EventState.Voting)
                return Unauthorized();

            Entry[] entries = DatabaseHelpers.Entries.GetEntriesInEvent(currentEvent);

            Random rnd = new Random();
            Entry[] randomizedEntries = entries.OrderBy(e => rnd.Next()).ToArray();

            ViewBag.Event = currentEvent;
            ViewBag.RandomizedEntries = randomizedEntries;

            return View(currentEvent);
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

            _logger.LogInformation("User {0} cast a vote on the entry with ID {1}.", user.Username, entryId);

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

            _logger.LogInformation("User {0} removed their vote on the entry with ID {1}.", user.Username, entryId);

            DatabaseHelpers.Votes.DeleteVote(entryId, user.UserId);
            return Ok(new VoteActionResult(true, false));
        }
    }
}