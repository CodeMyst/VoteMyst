using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using VoteMyst.Database;

namespace VoteMyst.Controllers
{
    public class EventController : Controller
    {
        private readonly DatabaseHelperProvider _helpers;

        public EventController(DatabaseHelperProvider helpers) 
        {
            _helpers = helpers;
        }

        public IActionResult Browse()
        {
            return View();
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

        [HttpPost]
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