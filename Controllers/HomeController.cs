using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace VoteMyst.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            // If an event is currently happening, display information about it
            if (true) 
            {
                return View("DisplayCurrent");
            }
            
            // If an event is coming up, display information about it
            if (true)
            {
                return View("DisplayUpcoming");
            }

            return View("DisplayNone");
        }
    }
}