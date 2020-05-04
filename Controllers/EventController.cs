using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using VoteMyst.Database;
<<<<<<< HEAD
using VoteMyst.Database.Models;
=======
using VoteMyst.PermissionSystem;
>>>>>>> 01d207bf6fdaf8d089312876fd3034d54ddab0ad

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
<<<<<<< HEAD
            Event[] finishedEvents = eventHelper.GetAllEventsFinishedBefore(DateTime.UtcNow);
            Event[] plannedEvents = eventHelper.GetVisiblePlannedEvents();
            Event[] ongoingEvents = eventHelper.GetCurrentEvents();

            ViewBag.History = finishedEvents;
            ViewBag.Planned = plannedEvents;
            ViewBag.Current = ongoingEvents;

            return View(nameof(Browse));
=======
            return View();
>>>>>>> 01d207bf6fdaf8d089312876fd3034d54ddab0ad
        }

        [RequirePermissions(Permissions.ViewEntries)]
        public IActionResult Display(int userId) 
        {
            ViewBag.ID = userId;
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