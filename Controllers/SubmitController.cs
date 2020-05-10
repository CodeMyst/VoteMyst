using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.StaticFiles;

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

            if (DateTime.UtcNow < currentEvent.StartDate || DateTime.UtcNow > currentEvent.EndDate)
                return NotFound();

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
            if (file == null)
                return ShowSubmissionError("Something needs to be uploaded.");

            if (file.Length > MaxFileSize) 
                return ShowSubmissionError("The file is too large.");
            
            // TODO: Maybe support multiple events?
            Event currentEvent = _helpers.Events.GetCurrentEvents().FirstOrDefault();
            if (currentEvent == null)
                return NotFound();

            // Make sure that the type to upload is valid
            var typeProvider = new FileExtensionContentTypeProvider();
            if (!typeProvider.TryGetContentType(file.FileName, out string contentType))
                return ShowSubmissionError("Unknown file type.");

            if (currentEvent.EventType == EventType.Art && !contentType.StartsWith("image/"))
                return ShowSubmissionError("Only image files are allowed for this event.");

            // TODO: Validate other event types

            UserData user = _profileBuilder.FromPrincipal(User);

            Entry existingEntry = _helpers.Entries.GetEntryOfUserInEvent(currentEvent, user);
            if (existingEntry != null)
            {
                _helpers.Entries.DeleteEntry(existingEntry);

                if (existingEntry.EntryType == EntryType.File)
                {
                    // Retrieve the previous entry path, making sure to make the path relative instead of absolute
                    string entryAssetPath = Path.Combine(_environment.WebRootPath, existingEntry.Content.Substring(1));
                    if (System.IO.File.Exists(entryAssetPath)) 
                    {
                        System.IO.File.Delete(entryAssetPath);
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

            Entry entry = _helpers.Entries.CreateEntry(currentEvent, user, EntryType.File, "/" + relativePath);

            ViewBag.SubmitSuccessful = true;
            ViewBag.Event = currentEvent;
            ViewBag.Entry = entry;

            return View();
        }

        private IActionResult ShowSubmissionError(string message)
        {
            ViewBag.ErrorMessage = message;
            return Index();
        }
    }
}