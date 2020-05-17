using System;
using System.IO;
using System.Linq;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Logging;

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

        private readonly ILogger _logger;

        public SubmitController(ILogger<SubmitController> logger, IServiceProvider serviceProvider) : base(serviceProvider) 
        { 
            _logger = logger;
        }

        /// <summary>
        /// Shows the submission page for the event with the specified ID.
        /// </summary>
        [Route("submit/{id}")]
        [RequirePermissions(Permissions.SubmitEntries)]
        public IActionResult Index(string id)
        {
            UserData currentUser = GetCurrentUser();
            Event currentEvent = DatabaseHelpers.Events.GetEvent(id);

            bool validEvent = SetupValidation(validator => 
            {
                validator.Verify(currentEvent != null);
                validator.Verify(currentEvent.GetCurrentState() == EventState.Ongoing);
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
        public IActionResult Index(string id, IFormFile file)
        {
            // TODO: Maybe support multiple events?
            Event ev = DatabaseHelpers.Events.GetEvent(id);
            
            bool validEvent = SetupValidation(validator => 
            {
                validator.Verify(ev != null);
                validator.Verify(ev.GetCurrentState() == EventState.Ongoing);
            }).Run();

            if (!validEvent)
                return NotFound();

            bool validForm = SetupValidation(validator =>
            {
                validator.Verify(file != null, "A file to be uploaded needs to be specified.");
                validator.Verify(file.Length <= MaxFileMB * 1000000, "The specified file is too large.");

                var typeProvider = new FileExtensionContentTypeProvider();
                validator.Verify(typeProvider.TryGetContentType(file.FileName, out string contentType), "Unknown file type.");

                switch(ev.EventType)
                {
                    case EventType.Art: validator.Verify(contentType.StartsWith("image/"), "Only image files are allowed for this event."); break;
                    // TODO: Validate other event types
                }

            }).HandleInvalid(InjectResultIntoView).Run();

            if (validForm)
            {
                UserData user = GetCurrentUser();

                Entry existingEntry = DatabaseHelpers.Entries.GetEntryOfUserInEvent(ev, user);
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

                string relativePath = PathBuilder.GetEntryUrlRelative(ev, user, file.FileName);
                string absolutePath = PathBuilder.GetEntryUrlAbsolute(ev, user, file.FileName);

                // Ensure that the directory exists
                Directory.CreateDirectory(Path.GetDirectoryName(absolutePath));

                using (var localFile = System.IO.File.OpenWrite(absolutePath))
                using (var uploadedFile = file.OpenReadStream())
                {
                    uploadedFile.CopyTo(localFile);
                }

                Entry entry = DatabaseHelpers.Entries.CreateEntry(ev, user, EntryType.File, relativePath);

                _logger.LogInformation("User {0} uploaded a submission to the event '{1}'.", user.Username, ev.Title);

                return Redirect("/events/display/" + ev.Url);
            }
            
            return Index(id);
        }
    }
}