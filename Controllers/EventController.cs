using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using VoteMyst.Database;
using VoteMyst.Database.Models;
using VoteMyst.PermissionSystem;
namespace VoteMyst.Controllers
{
    public class EventController : Controller
    {
        private readonly DatabaseHelperProvider _helpers;

        public EventController(DatabaseHelperProvider helpers) 
        {
            _helpers = helpers;
        }

        [RequirePermissions(Permissions.ViewEvents)]
        public IActionResult Browse()
        {
            Event[] finishedEvents = _helpers.Events.GetAllEventsFinishedBefore(DateTime.UtcNow);
            Event[] plannedEvents = _helpers.Events.GetVisiblePlannedEvents();
            Event[] ongoingEvents = _helpers.Events.GetCurrentEvents();

            ViewBag.History = finishedEvents;
            ViewBag.Planned = plannedEvents;
            ViewBag.Current = ongoingEvents;

            return View(nameof(Browse));
        }

        [RequirePermissions(Permissions.ViewEntries)]
        public IActionResult Display(int eventId) 
        {
            Event e = _helpers.Events.GetEvent(eventId);

            if (e == null)
                return NotFound();

            bool beforeReveal = DateTime.Now < e.RevealDate;
            bool beforeEnd = DateTime.UtcNow < e.EndDate;
            bool inVote = DateTime.UtcNow > e.EndDate && DateTime.UtcNow < e.VoteEndDate;
            bool afterVoteEnd = DateTime.UtcNow > e.VoteEndDate;

            if (beforeReveal)
                return NotFound();

            if (inVote) 
            {
                // If the event voting phase has started and not ended yet, redirect to the vote page
                return Redirect("/vote");
            }

            // Otherwise display general event information
            ViewBag.Event = e;

            if (beforeEnd)
            {
                ViewBag.Entries = _helpers.Entries.GetEntriesInEvent(e);
            }
            if (afterVoteEnd)
            {
                // If voting has ended, display the winners in order
                ViewBag.Leaderboard = _helpers.Entries.GetEntriesInEvent(e)
                    .Select(e => (_helpers.Votes.GetAllVotesForEntry(e).Length, e))
                    .OrderByDescending(e => e.Item1)
                    .ThenBy(e => e.Item2.EntryId)
                    .ToArray();
            }

            return View();
        }

        [RequirePermissions(Permissions.CreateEvents)]
        public IActionResult New() 
        {
            return View();
        }

        [HttpPost]
        [RequirePermissions(Permissions.CreateEvents)]
        public IActionResult New(
            string title, string description, EventType eventType,
            DateTime revealDate, DateTime startDate,
            DateTime endDate, DateTime voteEndDate)
        {
            _helpers.Events.CreateEvent(title, description, eventType,
                revealDate, startDate, endDate, voteEndDate);

            ViewBag.SuccessfullyCreated = true;
            return View();
        }
    }
}