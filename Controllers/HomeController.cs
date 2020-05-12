using System;

using Microsoft.AspNetCore.Mvc;

using VoteMyst.Database;
using VoteMyst.Database.Models;

namespace VoteMyst.Controllers
{
    /// <summary>
    /// Provides the homepage for the site.
    /// </summary>
    public class HomeController : VoteMystController
    {
        public HomeController(IServiceProvider serviceProvider) : base(serviceProvider) { }

        /// <summary>
        /// The index page for the site.
        /// </summary>
        [Route("")]
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult IndexOLD()
        {
            Event[] currentEvents = DatabaseHelpers.Events.GetCurrentEvents();

            // If an event is currently happening, display information about it
            if (currentEvents.Length > 0) 
            {
                // TODO: Maybe support multiple events?
                Event currentEvent = currentEvents[0];

                ViewBag.Id = currentEvent.EventId;
                ViewBag.Name = currentEvent.Title;
                ViewBag.Description = currentEvent.Description;
                ViewBag.SubmissionEnd = currentEvent.EndDate;
                ViewBag.VoteEnd = currentEvent.VoteEndDate;
                ViewBag.TotalEntries = DatabaseHelpers.Entries.GetEntriesInEvent(currentEvent).Length;
                ViewBag.TotalVotes = DatabaseHelpers.Votes.GetAllVotesInEvent(currentEvent).Length;

                return View("DisplayCurrent");
            }
            
            Event[] upcomingEvents = DatabaseHelpers.Events.GetAllEventsAfter(DateTime.UtcNow);

            // If an event is coming up, display information about it
            if (upcomingEvents.Length > 0)
            {
                return View("DisplayUpcoming");
            }

            return View("DisplayNone");
        }
    }
}