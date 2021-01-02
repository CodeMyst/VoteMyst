using System;
using System.Linq;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

using VoteMyst.Database;
using VoteMyst.Controllers.Api.Models;

namespace VoteMyst.Controllers.Api
{
    public class ReportApiController : VoteMystApiController
    {
        public ReportApiController(IServiceProvider serviceProvider) : base(serviceProvider) { }

        [HttpPost]
        [Route("api/reports/submit")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public IActionResult SubmitReport([FromBody] ReportSubmission reportSubmission)
        {
            if (!User.Identity.IsAuthenticated)
                return Unauthorized();

            Entry targetEntry = DatabaseHelpers.Context.QueryByDisplayID<Entry>(reportSubmission.EntryDisplayId);
            if (targetEntry == null)
                return BadRequest("The specified entry could not be found.");

            UserAccount currentUser = GetCurrentUser();

            if (currentUser.ID == targetEntry.Author.ID)
                return BadRequest("Reporting own posts is not allowed.");
            if (targetEntry.Reports.Any(r => r.ReportAuthor.ID == currentUser.ID))
                return BadRequest("A report on this entry by the current user already exists.");

            DatabaseHelpers.Entries.ReportEntry(targetEntry, currentUser, reportSubmission.Reason);

            return Ok();
        }

        [HttpPost]
        [Route("api/reports/{id}/approve")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public IActionResult DeleteReportedPost(int id)
            => ProcessReportAction(id, ReportStatus.Approved);

        [HttpPost]
        [Route("api/reports/{id}/reject")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public IActionResult RejectReport(int id)
            => ProcessReportAction(id, ReportStatus.Rejected);

        private IActionResult ProcessReportAction(int reportId, ReportStatus desiredStatus)
        {
            if (desiredStatus == ReportStatus.Pending)
                throw new ArgumentException("The desired status 'Pending' cannot be processed.");

            UserAccount user = GetCurrentUser();

            Report r = DatabaseHelpers.Context.QueryByID<Report>(reportId);
            if (r == null)
                return BadRequest("The specified report could not be found.");
            if (r.Status != ReportStatus.Pending)
                return BadRequest("The report is not pending and cannot be processed further.");

            EventPermissions permissions = DatabaseHelpers.Events.GetUserPermissionsForEvent(user, r.Entry.Event);
            if (!permissions.HasFlag(EventPermissions.ManageEntries) && !user.Permissions.HasFlag(GlobalPermissions.ManageAllEvents))
                return Unauthorized();

            if (desiredStatus == ReportStatus.Approved)
            {
                // Approve the report; delete the reported entry
                DatabaseHelpers.Entries.DeleteEntry(r.Entry);
            }
            DatabaseHelpers.Entries.UpdateEntryReportStatus(r, desiredStatus);

            return Ok();
        }
    }
}
