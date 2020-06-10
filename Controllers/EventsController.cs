using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

using VoteMyst.Database;
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
            public class Place 
            {
                public int Number { get; set; }
                public UserAccount Author { get; set; }
                public Entry Entry { get; set; }
                public int Votes { get; set; }

                public static Place FromEntry(Entry entry, DatabaseHelperProvider helpers)
                {
                    return new Place
                    {
                        Number = -1,
                        Author = entry.Author,
                        Entry = entry,
                        Votes = entry.Votes.Count
                    };
                }
            }

            public Entry[] NotEligable { get; }
            public Place[] Places { get; }

            private Leaderboard(Place[] places, Entry[] notEligable = null)
            {
                Places = places;
                NotEligable = notEligable;
            }

            public static Leaderboard FromEvent(Event ev, DatabaseHelperProvider helpers)
            {
                Place[] places = ev.Entries
                    .Where(entry => helpers.Events.CanUserWin(entry.Author, ev))
                    .Select(entry => Place.FromEntry(entry, helpers))
                    .OrderByDescending(place => place.Votes)
                    .ThenBy(place => place.Entry.ID)
                    .ToArray();

                Entry[] notEligable = ev.Entries
                    .Where(entry => !helpers.Events.CanUserWin(entry.Author, ev))
                    .ToArray();

                int currentPlace = 0;
                int currentVotes = -1;

                for(int i = 0; i < places.Length; i++)
                {
                    if (places[i].Votes != currentVotes)
                    {
                        currentVotes = places[i].Votes;
                        currentPlace++;
                    }

                    places[i].Number = currentPlace;
                }

                return new Leaderboard(places, notEligable);
            }

            public IEnumerator<Place> GetEnumerator()
            {
                foreach (Place p in Places)
                    yield return p;
            }
            IEnumerator IEnumerable.GetEnumerator()
                => Places.GetEnumerator();
        }

        private readonly ILogger _logger;

        public EventsController(ILogger<EventsController> logger, IServiceProvider serviceProvider) : base(serviceProvider)
        {
            _logger = logger;
        }

        /// <summary>
        /// Displays an overview of all events.
        /// </summary>
        public IActionResult Index()
        {
            UserAccount user = GetCurrentUser();

            ViewBag.CanManageEvents = user.Permissions.HasFlag(GlobalPermissions.CreateEvents);

            return View(DatabaseHelpers.Events.GetAllEventsGrouped());
        }

        /// <summary>
        /// Displays the event with the specified ID.
        /// </summary>
        public IActionResult Display(string id) 
        {
            UserAccount user = GetCurrentUser();
            Event e = DatabaseHelpers.Events.GetEventByUrl(id);
            ViewBag.Event = e;

            if (e == null)
                return NotFound();
                
            EventState eventState = e.GetCurrentState();

            // If the event is not revealed yet, don't allow to find it, except if the user is an admin
            if (eventState == EventState.Hidden && user.AccountBadge == AccountBadge.SiteAdministrator)
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
        [RequireGlobalPermission(GlobalPermissions.CreateEvents)]
        public IActionResult New() 
        {
            return View(new Event());
        }

        /// <summary>
        /// Creates a new event in the database.
        /// </summary>
        [HttpPost]
        [RequireGlobalPermission(GlobalPermissions.CreateEvents)]
        public IActionResult New([FromForm] Event e)
        {
            if (string.IsNullOrEmpty(e.URL))
            {
                e.URL = Regex.Replace(Regex.Replace(e.Title.ToLowerInvariant().Replace(" ", "-"), 
                    @"[^a-zA-Z\d\-]", string.Empty).Trim('-'), @"-{2,}", "-");
            }

            // Temporarily set the DisplayID to something to bypass user validation.
            // The ID will later be injected when creating the event.
            e.DisplayID = "null";

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
                DatabaseHelpers.Events.CreateEvent(e, GetCurrentUser());

                _logger.LogInformation("User {0} created the event '{1}'.", GetCurrentUser().Username, e.Title);

                return View("Success", e);
            }
        }

        public IActionResult Vote(string id) 
        {
            // TODO: Maybe support multiple events?
            Event currentEvent = DatabaseHelpers.Events.GetEventByUrl(id);

            if (currentEvent == null)
                return NotFound();
            if (currentEvent.GetCurrentState() != EventState.Voting)
                return Unauthorized();

            Entry[] entries = currentEvent.Entries.ToArray();

            Random rnd = new Random();
            Entry[] randomizedEntries = entries.OrderBy(e => rnd.Next()).ToArray();

            ViewBag.Event = currentEvent;
            ViewBag.RandomizedEntries = randomizedEntries;

            return View(currentEvent);
        }

        [HttpPost]
        [Route("vote/cast/{entryDisplayId}")]
        public IActionResult CastVote(string entryDisplayId)
        {
            // Disallow anonymous voting
            if (!User.Identity.IsAuthenticated)
                return Forbid();

            Entry entry = DatabaseHelpers.Context.QueryByDisplayID<Entry>(entryDisplayId);
            Event entryEvent = entry.Event;

            UserAccount user = GetCurrentUser();
            UserAccount author = entry.Author;
            
            // Disallow voting on own posts
            if (user.ID == author.ID)
                return Unauthorized();

            // Only allow voting while its open
            if (entryEvent.GetCurrentState() != EventState.Voting)
                return Unauthorized();

            Vote vote = DatabaseHelpers.Votes.GetVoteByUserOnEntry(user, entry);
            Console.WriteLine(vote);

            // Make sure a vote by the user on the specified entry does not exist yet
            if (vote != null)
                return Ok(new VoteActionResult(false, true));
            
            // If all checks passed, cast the vote
            DatabaseHelpers.Votes.AddVote(entry, user);

            _logger.LogInformation("User {0} cast a vote on the entry with ID {1}.", user.Username, entryDisplayId);

            return Ok(new VoteActionResult(true, true));
        }
        
        [HttpPost]
        [Route("vote/remove/{entryDisplayId}")]
        public IActionResult RemoveVote(string entryDisplayId)
        {
            // Disallow anonymous voting
            if (!User.Identity.IsAuthenticated)
                return Forbid();
                
            UserAccount user = GetCurrentUser();
            Entry entry = DatabaseHelpers.Context.QueryByDisplayID<Entry>(entryDisplayId);
            Vote vote = DatabaseHelpers.Votes.GetVoteByUserOnEntry(user, entry);
            Event entryEvent = entry.Event;

            // Only allow deleting votes if voting is still open
             if (entryEvent.GetCurrentState() != EventState.Voting)
                return Unauthorized();

            // Make sure a vote exists that can be deleted
            if (vote == null)
                return Ok(new VoteActionResult(false, false));

            _logger.LogInformation("User {0} removed their vote on the entry with ID {1}.", user.Username, entryDisplayId);

            DatabaseHelpers.Votes.DeleteVote(vote);
            return Ok(new VoteActionResult(true, false));
        }
    }
}