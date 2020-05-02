using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using VoteMyst.Database;
using VoteMyst.Database.Models;

namespace VoteMyst.Controllers
{
    public class EventController : Controller
    {

        private readonly EventHelper eventHelper;
        public EventController(EventHelper eventHelper) 
        {
            this.eventHelper = eventHelper;
        }

        public IActionResult Browse()
        {
            Event[] finishedEvents = eventHelper.GetAllEventsFinishedBefore(DateTime.UtcNow);
            Event[] plannedEvents = eventHelper.GetVisiblePlannedEvents();
            Event[] ongoingEvents = eventHelper.GetCurrentEvents();

            ViewBag.History = finishedEvents;
            ViewBag.Planned = plannedEvents;
            ViewBag.Current = ongoingEvents;

            return View(nameof(Browse));
        }

        public IActionResult Display(int userId) 
        {
            ViewBag.ID = userId;
            return View();
        }

        public IActionResult New() 
        {
            return View();
        }
    }
}