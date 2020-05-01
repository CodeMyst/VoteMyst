using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace VoteMyst.Controllers
{
    public class EventController : Controller
    {
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
    }
}