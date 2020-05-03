using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;

using VoteMyst.Database;
using VoteMyst.Database.Models;

namespace VoteMyst.Controllers
{
    public class SubmitController : Controller
    {
        public const int MaxFileSize = 4000000;

        private readonly UserProfileBuilder _profileBuilder;
        private readonly DatabaseHelperProvider _helpers;
        private readonly IWebHostEnvironment _environment;

        public SubmitController(UserProfileBuilder profileBuilder, DatabaseHelperProvider helpers, IWebHostEnvironment environment) 
        {
            _profileBuilder = profileBuilder;
            _helpers = helpers;
            _environment = environment;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Index(IFormFile file)
        {
            if (file.Length > MaxFileSize) {
                ViewBag.Message = "File too large!";
                return View();
            }

            Event[] currentEvents = _helpers.Events.GetCurrentEvents();
            if (currentEvents.Length == 0)
                return NotFound();

            // TODO: Maybe support multiple events?

            Event currentEvent = currentEvents[0];
            UserData user = _profileBuilder.FromContext(HttpContext);

            string fileName = Path.GetFileName(file.FileName);
            string relativePath = $"assets/events/{currentEvent.EventId}/{user.DisplayId}{Path.GetExtension(fileName)}";
            string absolutePath = Path.Combine(_environment.WebRootPath, relativePath);

            Directory.CreateDirectory(Path.GetDirectoryName(absolutePath));

            using (var localFile = System.IO.File.OpenWrite(absolutePath))
            using (var uploadedFile = file.OpenReadStream())
            {
                uploadedFile.CopyTo(localFile);
            }

            _helpers.Entries.CreateEntry(currentEvent, user, EntryType.File, relativePath);

            ViewBag.Message = "Your entry was submitted!";

            return View();
        }
    }
}