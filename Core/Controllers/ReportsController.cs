using System;
using System.IO;
using System.Linq;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Logging;

using VoteMyst.Database;
using VoteMyst.PermissionSystem;

namespace VoteMyst.Controllers
{
    /// <summary>
    /// Provides a controller that handles reports of entries.
    /// </summary>
    public class ReportsController : VoteMystController
    {
        public class ReportSubmission
        {
            public string EntryDisplayId { get; set; }
            public string Reason { get; set; }
        }

        private readonly ILogger _logger;

        public ReportsController(ILogger<SubmitController> logger, IServiceProvider serviceProvider) : base(serviceProvider) 
        { 
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [Route("/reports/submit")]
        public IActionResult SubmitReport([FromBody] ReportSubmission reportSubmission)
        {
            Entry targetEntry = DatabaseHelpers.Context.QueryByDisplayID<Entry>(reportSubmission.EntryDisplayId);
            UserAccount currentUser = GetCurrentUser();

            if (currentUser.ID == targetEntry.Author.ID)
                return BadRequest("Reporting own posts is not allowed.");

            DatabaseHelpers.Entries.ReportEntry(targetEntry, currentUser, reportSubmission.Reason);

            return Ok();
        }

        [HttpPost]
        [Route("/reports/action/delete")]
        public IActionResult DeleteReportedPost([FromBody] int reportId)
        {
            Report r = DatabaseHelpers.Context.QueryByID<Report>(reportId);
            EventPermissions permissions = DatabaseHelpers.Events.GetUserPermissionsForEvent(GetCurrentUser(), r.Entry.Event);

            if (!permissions.HasFlag(EventPermissions.ManageEntries))
                return Unauthorized();

            // Note: This will also delete the report (and all votes linked to the entry), so the "approved" status will never actually be visible.
            DatabaseHelpers.Entries.DeleteEntry(r.Entry);
            DatabaseHelpers.Entries.UpdateEntryReportStatus(r, ReportStatus.Approved);

            return Ok();
        }

        [HttpPost]
        [Route("/reports/action/reject")]
        public IActionResult RejectReport([FromBody] int reportId)
        {
            Report r = DatabaseHelpers.Context.QueryByID<Report>(reportId);
            EventPermissions permissions = DatabaseHelpers.Events.GetUserPermissionsForEvent(GetCurrentUser(), r.Entry.Event);

            if (!permissions.HasFlag(EventPermissions.ManageEntries))
                return Unauthorized();

            DatabaseHelpers.Entries.UpdateEntryReportStatus(r, ReportStatus.Rejected);

            return Ok();
        }
    }
}