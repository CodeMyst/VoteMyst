using System;
using System.IO;
using System.Linq;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.StaticFiles;

using VoteMyst.Database;
using VoteMyst.Database.Models;
using VoteMyst.PermissionSystem;

namespace VoteMyst.Controllers
{
    /// <summary>
    /// Provides a controller that handles submissions to events.
    /// </summary>
    public class SubmitController : VoteMystController
    {
        /// <summary>
        /// The maximum size that a file can have (in megabytes).
        /// </summary>
        public const int MaxFileMB = 4;

        public SubmitController(IServiceProvider serviceProvider) : base(serviceProvider) { }

        /// <summary>
        /// Shows the submission page for the event with the specified ID.
        /// </summary>
        [Route("submit/{id}")]
        [RequirePermissions(Permissions.SubmitEntries)]
        public IActionResult Index(int id)
        {
            UserData currentUser = GetCurrentUser();
            Event currentEvent = DatabaseHelpers.Events.GetCurrentEvents().FirstOrDefault(e => e.EventId == id);

            bool validEvent = SetupValidation(validator => 
            {
                validator.Verify(currentEvent != null);
                validator.Verify(DateTime.UtcNow >= currentEvent.StartDate && DateTime.UtcNow < currentEvent.EndDate);
                // TODO: Switch to event state checking
            }).Run();

            if (!validEvent)
                return NotFound();

            ViewBag.MaxFileMB = MaxFileMB;
            ViewBag.Event = currentEvent;

            Entry currentEntry = DatabaseHelpers.Entries.GetEntryOfUserInEvent(currentEvent, currentUser);
            ViewBag.Entry = currentEntry;

            return View();
        }

        /// <summary>
        /// Provides the endpoint for submission POST requests.
        /// </summary>
        [HttpPost]
        [Route("submit/{id}")]
        [RequirePermissions(Permissions.SubmitEntries)]
        public IActionResult Index(int id, IFormFile file)
        {
            // TODO: Maybe support multiple events?
            Event currentEvent = DatabaseHelpers.Events.GetCurrentEvents().FirstOrDefault(e => e.EventId == id);
            
            bool validEvent = SetupValidation(validator => 
            {
                validator.Verify(currentEvent != null);
                validator.Verify(DateTime.UtcNow >= currentEvent.StartDate && DateTime.UtcNow < currentEvent.EndDate);
                // TODO: Switch to event state
            }).Run();

            if (!validEvent)
                return NotFound();

            bool validForm = SetupValidation(validator =>
            {
                validator.Verify(file != null, "A file to be uploaded needs to be specified.");
                validator.Verify(file.Length <= MaxFileMB * 1000000, "The specified file is too large.");

                var typeProvider = new FileExtensionContentTypeProvider();
                validator.Verify(typeProvider.TryGetContentType(file.FileName, out string contentType), "Unknown file type.");
                validator.Verify(currentEvent.EventType == EventType.Art && contentType.StartsWith("image/"), "Only image files are allowed for this event.");
                // TODO: Validate other event types

            }).HandleInvalid(InjectResultIntoView).Run();

            if (validForm)
            {
                UserData user = GetCurrentUser();

                Entry existingEntry = DatabaseHelpers.Entries.GetEntryOfUserInEvent(currentEvent, user);
                if (existingEntry != null)
                {
                    DatabaseHelpers.Entries.DeleteEntry(existingEntry);

                    if (existingEntry.EntryType == EntryType.File)
                    {
                        // Retrieve the previous entry path, making sure to make the path relative instead of absolute
                        string entryAssetPath = PathBuilder.GetEntryUrl(existingEntry);

                        if (System.IO.File.Exists(entryAssetPath)) 
                        {
                            System.IO.File.Delete(entryAssetPath);
                        }
                    }
                }

                string relativePath = PathBuilder.GetEntryUrlRelative(currentEvent, user, file.FileName);
                string absolutePath = PathBuilder.GetEntryUrlAbsolute(currentEvent, user, file.FileName);

                // Ensure that the directory exists
                Directory.CreateDirectory(Path.GetDirectoryName(absolutePath));

                using (var localFile = System.IO.File.OpenWrite(absolutePath))
                using (var uploadedFile = file.OpenReadStream())
                {
                    uploadedFile.CopyTo(localFile);
                }

                Entry entry = DatabaseHelpers.Entries.CreateEntry(currentEvent, user, EntryType.File, relativePath);
                ViewBag.UploadedEntry = entry;
            }
            
            return Index(id);
        }
    }
}