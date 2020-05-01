using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace VoteMyst.Controllers
{
    public class UserController : Controller
    {
        public IActionResult Display(int userId) 
        {
            ViewBag.ID = userId;
            return View(nameof(Display));
        }

        public IActionResult DisplaySelf()
        {
            return Display(-1);
        }

        public IActionResult BanUser(int id)
        {
            return Redirect((id + 3).ToString());
        }
    }
}