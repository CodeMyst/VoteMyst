using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using VoteMyst.Database;
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
            return View();
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