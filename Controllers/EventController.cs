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

        private readonly EventHelper eventHelper;
        public EventController(EventHelper eventHelper) 
        {
            this.eventHelper = eventHelper;
        }

        public IActionResult Browse()
        {
            Console.WriteLine(eventHelper);
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
    }
}