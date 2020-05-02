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
        private readonly EventHelper _eventHelper;
        private readonly EntryHelper _entryHelper;
        private readonly VoteHelper _voteHelper;

        public HomeController(EventHelper eventHelper, EntryHelper entryHelper, VoteHelper voteHelper) 
        {
            _eventHelper = eventHelper;
            _entryHelper = entryHelper;
            _voteHelper = voteHelper;
        }

        public IActionResult Index()
        {
            Event[] currentEvents = _eventHelper.GetCurrentEvents();

            // If an event is currently happening, display information about it
            if (currentEvents.Length > 0) 
            {
                // TODO: Maybe support multiple events?
                Event currentEvent = currentEvents[0];

                ViewBag.Name = currentEvent.Title;
                ViewBag.Description = currentEvent.Description;
                ViewBag.SubmissionEnd = currentEvent.EndDate;
                ViewBag.VoteEnd = currentEvent.VoteEndDate;
                ViewBag.TotalEntries = _entryHelper.GetEntriesInEvent(currentEvent).Length;
                ViewBag.TotalVotes = _voteHelper.GetAllVotesInEvent(currentEvent).Length;

                return View("DisplayCurrent");
            }
            
            Event[] upcomingEvents = _eventHelper.GetAllEventsAfter(DateTime.UtcNow);

            // If an event is coming up, display information about it
            if (upcomingEvents.Length > 0)
            {
                return View("DisplayUpcoming");
            }

            return View("DisplayNone");
        }
    }
}