using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace VoteMyst.Controllers
{
    public class VoteController : Controller
    {
        public IActionResult Index() 
        {
            return View();
        }
    }
}