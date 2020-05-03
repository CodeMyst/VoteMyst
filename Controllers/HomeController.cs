using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

using VoteMyst.Database;
using VoteMyst.Database.Models;

namespace VoteMyst.Controllers
{
    public class HomeController : Controller
    {
        private readonly DatabaseHelperProvider _helpers;

        public HomeController(DatabaseHelperProvider helpers) 
        {
            _helpers = helpers;
        }

        public IActionResult Index()
        {
            Event[] currentEvents = _helpers.Events.GetCurrentEvents();

            // If an event is currently happening, display information about it
            if (currentEvents.Length > 0) 
            {
                // TODO: Maybe support multiple events?
                Event currentEvent = currentEvents[0];

                ViewBag.Name = currentEvent.Title;
                ViewBag.Description = currentEvent.Description;
                ViewBag.SubmissionEnd = currentEvent.EndDate;
                ViewBag.VoteEnd = currentEvent.VoteEndDate;
                ViewBag.TotalEntries = _helpers.Entries.GetEntriesInEvent(currentEvent).Length;
                ViewBag.TotalVotes = _helpers.Votes.GetAllVotesInEvent(currentEvent).Length;

                return View("DisplayCurrent");
            }
            
            Event[] upcomingEvents = _helpers.Events.GetAllEventsAfter(DateTime.UtcNow);

            // If an event is coming up, display information about it
            if (upcomingEvents.Length > 0)
            {
                return View("DisplayUpcoming");
            }

            return View("DisplayNone");
        }
    }
}