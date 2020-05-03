using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

using VoteMyst.Database;
using VoteMyst.Database.Models;

namespace VoteMyst.Controllers
{
    public class VoteController : Controller
    {
        private readonly DatabaseHelperProvider _helpers;

        public VoteController(DatabaseHelperProvider helpers)
        {
            _helpers = helpers;
        }

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
    }
}