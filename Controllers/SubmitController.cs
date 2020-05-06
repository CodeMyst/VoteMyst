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
using VoteMyst.PermissionSystem;

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

        [RequirePermissions(Permissions.SubmitEntries)]
        public IActionResult Index()
        {
            UserData currentUser = _profileBuilder.FromPrincipal(User);
            Event currentEvent = _helpers.Events.GetCurrentEvents().FirstOrDefault();
            ViewBag.Event = currentEvent;

            if (currentEvent != null)
            {
                Entry currentEntry = _helpers.Entries.GetEntryOfUserInEvent(currentEvent, currentUser);
                ViewBag.Entry = currentEntry;
            }

            return View();
        }

        [HttpPost]
        [RequirePermissions(Permissions.SubmitEntries)]
        public IActionResult Index(IFormFile file)
        {
            ViewBag.SubmitAttempted = true;

            if (file.Length > MaxFileSize) 
            {
                ViewBag.SubmitFileTooLarge = true;
                return View();
            }

            Event[] currentEvents = _helpers.Events.GetCurrentEvents();
            if (currentEvents.Length == 0)
                return NotFound();

            // TODO: Maybe support multiple events?

            Event currentEvent = currentEvents[0];
            UserData user = _profileBuilder.FromPrincipal(User);

            Entry existingEntry = _helpers.Entries.GetEntryOfUserInEvent(currentEvent, user);
            if (existingEntry != null)
            {
                _helpers.Entries.DeleteEntry(existingEntry);

                // Ensure that all previous submission files are removed
                foreach (string submissionFile in Directory.GetFiles(Path.Combine(_environment.WebRootPath, $"assets/events/{currentEvent.EventId}")))
                {
                    if (Path.GetFileNameWithoutExtension(submissionFile).StartsWith(user.DisplayId)) 
                    {
                        System.IO.File.Delete(submissionFile);
                    }
                }
            }

            string fileName = Path.GetFileName(file.FileName);
            string relativePath = $"assets/events/{currentEvent.EventId}/{user.DisplayId}{Path.GetExtension(fileName)}";
            string absolutePath = Path.Combine(_environment.WebRootPath, relativePath);

            Directory.CreateDirectory(Path.GetDirectoryName(absolutePath));

            using (var localFile = System.IO.File.OpenWrite(absolutePath))
            using (var uploadedFile = file.OpenReadStream())
            {
                uploadedFile.CopyTo(localFile);
            }

            Entry entry = _helpers.Entries.CreateEntry(currentEvent, user, EntryType.File, relativePath);

            ViewBag.SubmitSuccessful = true;
            ViewBag.Event = currentEvent;
            ViewBag.Entry = entry;

            return View();
        }
    }
}