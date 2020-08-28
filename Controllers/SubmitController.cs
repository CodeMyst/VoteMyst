using System;
using System.IO;
using System.Linq;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Logging;

using VoteMyst.Database;
using VoteMyst.Authorization;

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
        [Route("events/{id}/submit")]
        [CheckEventExists]
        [RequireGlobalPermission(GlobalPermissions.ParticipateInEvents)]
        [RequireEventPermission(EventPermissions.ParticipateInEvent)]
        public IActionResult Index(string id)
        {
            UserAccount currentUser = GetCurrentUser();
            Event currentEvent = DatabaseHelpers.Events.GetEventByUrl(id);

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
        [Route("events/{id}/submit")]
        [CheckEventExists]
        [RequireGlobalPermission(GlobalPermissions.ParticipateInEvents)]
        [RequireEventPermission(EventPermissions.ParticipateInEvent)]
        public IActionResult Index(string id, 
            [FromForm] IFormFile file, 
            [FromForm] string storyContent, 
            [FromForm] string pasteLink, 
            [FromForm] string itchLink)
        {
            // TODO: Maybe support multiple events?
            Event ev = DatabaseHelpers.Events.GetEventByUrl(id);

            bool validEvent = SetupValidation(validator => 
            {
                validator.Verify(ev != null);
                validator.Verify(ev.GetCurrentState() == EventState.Ongoing);
            }).Run();

            if (!validEvent)
                return NotFound();

            bool validForm = SetupValidation(validator =>
            {
                switch(ev.EventType)
                {
                    case EventType.Art:
                        validator.Verify(file != null, "A file to be uploaded needs to be specified.");
                        validator.Verify(file.Length <= MaxFileMB * 1048576, "The specified file is too large.");

                        var typeProvider = new FileExtensionContentTypeProvider();
                        validator.Verify(typeProvider.TryGetContentType(file.FileName, out string contentType), "Unknown file type.");
                        validator.Verify(contentType.StartsWith("image/"), "Only image files are allowed for this event.");

                        break;
                    case EventType.Story:
                        validator.Verify(!string.IsNullOrEmpty(storyContent), "A submission may not be empty.");

                        break;
                    case EventType.Gamejam:
                    case EventType.Coding:
                        // Validate URL
                        break;
                }

            }).HandleInvalid(InjectResultIntoView).Run();

            if (validForm)
            {
                UserAccount user = GetCurrentUser();

                Entry existingEntry = DatabaseHelpers.Entries.GetEntryOfUserInEvent(ev, user);
                if (existingEntry != null)
                {
                    if (existingEntry.EntryType == EntryType.File)
                    {
                        // Retrieve the previous entry path, making sure to make the path relative instead of absolute
                        string entryAssetPath = existingEntry.GetAbsoluteUrl(Environment);

                        if (System.IO.File.Exists(entryAssetPath)) 
                        {
                            System.IO.File.Delete(entryAssetPath);
                        }
                    }

                    DatabaseHelpers.Entries.DeleteEntry(existingEntry);
                }
                
                Entry entry = DatabaseHelpers.Entries.CreateFileEntry(ev, user, file, Environment);

                _logger.LogInformation("User {0} uploaded a submission to the event '{1}'.", user.Username, ev.Title);

                return Redirect(entry.GetUrl());
            }
            
            return Index(id);
        }
    }
}