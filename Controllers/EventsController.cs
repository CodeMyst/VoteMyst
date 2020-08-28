using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;

using VoteMyst.Database;
using VoteMyst.Authorization;
using VoteMyst.Controllers.Api;

namespace VoteMyst.Controllers
{
    /// <summary>
    /// Provides a controller that handles events.
    /// </summary>
    public class EventsController : VoteMystController
    {
        /// <summary>
        /// Represents a leaderboard for an event.
        /// </summary>
        public class Leaderboard : IEnumerable<Leaderboard.Place> 
        {
            /// <summary>
            /// Represents a place on the <see cref="Leaderboard"/>.
            /// </summary>
            public class Place 
            {
                /// <summary>
                /// The number that the place is at.
                /// </summary>
                public int Number { get; set; }
                /// <summary>
                /// The author of the entry at the place.
                /// </summary>
                public UserAccount Author { get; set; }
                /// <summary>
                /// The entry at the place.
                /// </summary>
                public Entry Entry { get; set; }
                /// <summary>
                /// The number of votes on the entry.
                /// </summary>
                public int Votes { get; set; }

                /// <summary>
                /// Creates a <see cref="Place"/> from an <see cref="VoteMyst.Database.Entry"/>.
                /// </summary>
                public static Place FromEntry(Entry entry)
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

            /// <summary>
            /// The entries that were not eligable to win.
            /// </summary>
            public Entry[] NotEligable { get; }
            /// <summary>
            /// The places on the leaderboard.
            /// </summary>
            public Place[] Places { get; }

            private Leaderboard(Place[] places, Entry[] notEligable = null)
            {
                Places = places;
                NotEligable = notEligable;
            }

            /// <summary>
            /// Creates a new leaderboard for the specified event.
            /// </summary>
            public static Leaderboard FromEvent(Event ev, DatabaseHelperProvider helpers)
            {
                Place[] places = ev.Entries
                    .Where(entry => helpers.Events.CanUserWin(entry.Author, ev))
                    .Select(entry => Place.FromEntry(entry))
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
        private readonly EventApiController _apiController;

        public EventsController(ILogger<EventsController> logger, IServiceProvider serviceProvider) : base(serviceProvider)
        {
            _logger = logger;
            _apiController = serviceProvider.GetService<EventApiController>();
        }

        /// <summary>
        /// Displays an overview of all events.
        /// </summary>
        public IActionResult Index()
        {
            return View(DatabaseHelpers.Events.GetAllEventsGrouped());
        }

        /// <summary>
        /// Displays the event with the specified ID.
        /// </summary>
        [Route("events/{id}")]
        public IActionResult Display(string id) 
        {
            UserAccount user = GetCurrentUser();
            Event e = DatabaseHelpers.Events.GetEventByUrl(id);

            if (e == null)
                return NotFound();
                
            EventState eventState = e.GetCurrentState();

            // If the event is not revealed yet, don't allow to find it, except if the user is a host or an admin
            if (eventState == EventState.Hidden && !DatabaseHelpers.Events.CanViewHiddenEvent(user, e))
            {
                _logger.LogWarning("{0} attempted to access the {1}, but it is hidden. Sending a 404 response.", user, e);
                return NotFound();
            }
            
            return View(e);
        }

        /// <summary>
        /// Displays the hosts of an event. Only available for event hosts.
        /// </summary>
        [Route("events/{id}/hosts")]
        [CheckEventExists]
        [RequireEventPermission(EventPermissions.EditEventSettings)]
        public IActionResult Hosts(string id)
        {
            Event e = DatabaseHelpers.Events.GetEventByUrl(id);
            return View(e);
        }

        /// <summary>
        /// Displays the settings of an event.
        /// </summary>
        [Route("events/{id}/settings")]
        [CheckEventExists]
        [RequireEventPermission(EventPermissions.EditEventSettings)]
        public IActionResult Settings(string id)
        {
            Event e = DatabaseHelpers.Events.GetEventByUrl(id);
            return View(e);
        }
        /// <summary>
        /// Provides the endpoint to modify event settings.
        /// </summary>
        [HttpPost]
        [Route("events/{id}/settings")]
        [CheckEventExists]
        [RequireEventPermission(EventPermissions.EditEventSettings)]
        public IActionResult Settings(string id, [FromForm, Bind] Event eventChanges)
        {
            UserAccount user = GetCurrentUser();
            Event targetEvent = DatabaseHelpers.Events.GetEventByUrl(id);
            if (targetEvent == null)
                return NotFound();

            EventPermissions userPermissions = DatabaseHelpers.Events.GetUserPermissionsForEvent(user, targetEvent);
            if (!userPermissions.HasFlag(EventPermissions.EditEventSettings) && !user.Permissions.HasFlag(GlobalPermissions.ManageAllEvents))
                return Forbid();

            eventChanges.ID = targetEvent.ID;

            ModelState.Clear();
            TryValidateModel(eventChanges);

            // Collect the initial model errors
            List<string> errorMessages = new List<string>();
            if (!ModelState.IsValid)
            {
                errorMessages = ModelState.Values.SelectMany(value => value.Errors).Select(error => error.ErrorMessage).ToList();
            }

            // Perform additional validation

            if (errorMessages.Count > 0)
            {
                // If validation errors occured, display them on the edit page.
                ViewBag.ErrorMessages = errorMessages.ToArray();
                return Settings(id);
            }

            targetEvent.Title = eventChanges.Title;
            targetEvent.URL = eventChanges.URL;
            targetEvent.Description = eventChanges.Description;

            targetEvent.EventType = eventChanges.EventType;
            targetEvent.Settings = eventChanges.Settings;

            targetEvent.RevealDate = eventChanges.RevealDate;
            targetEvent.StartDate = eventChanges.StartDate;
            targetEvent.SubmissionEndDate = eventChanges.SubmissionEndDate;
            targetEvent.VoteEndDate = eventChanges.VoteEndDate;

            DatabaseHelpers.Context.UpdateAndSave(targetEvent);

            return Redirect(targetEvent.GetUrl());
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
            if (string.IsNullOrEmpty(e.URL) && e.Title != null)
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

                return Redirect(e.GetUrl());
            }
        }

        [Route("events/{id}/reports")]
        [CheckEventExists]
        [RequireEventPermission(EventPermissions.ManageEntries)]
        public IActionResult Reports(string id)
        {
            Event e = DatabaseHelpers.Events.GetEventByUrl(id);
            return View(e);
        }
    }
}