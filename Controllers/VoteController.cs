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
        private readonly EventHelper _eventHelper;
        private readonly EntryHelper _entryHelper;
        private readonly VoteHelper _voteHelper;

        public VoteController(EventHelper eventHelper, EntryHelper entryHelper, VoteHelper voteHelper)
        {
            _eventHelper = eventHelper;
            _entryHelper = entryHelper;
            _voteHelper = voteHelper;
        }

        public IActionResult Index() 
        {
            Event[] currentEvents = _eventHelper.GetCurrentEvents();
            if (currentEvents.Length == 0)
                return NotFound();

            // TODO: Maybe support multiple events?

            Event currentEvent = currentEvents[0];
            Entry[] entries = _entryHelper.GetEntriesInEvent(currentEvent);

            Random rnd = new Random();
            Entry[] randomizedEntries = entries.OrderBy(e => rnd.Next()).ToArray();

            ViewBag.Entries = entries;
            ViewBag.RandomizedEntries = randomizedEntries;

            return View();
        }
    }
}