using System;
using System.IO;
using System.Linq;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;

namespace VoteMyst.Database
{
    /// <summary>
    /// Provides utility to handle <see cref="Entry"/>s.
    /// </summary>
    public class EntryHelper
    {
        private readonly VoteMystContext context;

        public EntryHelper(VoteMystContext context)
        {
            this.context = context;
        }

        /// <summary>
        /// Creates a new entry for the specified event, from the given user account.
        /// <para>Note that this method only creates the database entry.</para>
        /// </summary>
        public Entry CreateEntry(Event ev, UserAccount user, EntryType type, string content)
        {
            Entry entry = new Entry
            {
                Event = ev,
                Author = user,
                EntryType = type,
                Content = content,
                SubmitDate = DateTime.UtcNow
            };

            DisplayIDProvider.InjectDisplayId(entry);

            context.Entries.Add(entry);
            context.SaveChanges();

            return entry;
        }

        /// <summary>
        /// Creates a new file entry for the specified event, from the given user account. The file is also saved to the disk assets.
        /// </summary>
        public Entry CreateFileEntry(Event e, UserAccount user, IFormFile file, IWebHostEnvironment environment)
        {
            Entry entry = new Entry 
            {
                Event = e,
                Author = user,
                EntryType = EntryType.File,
                SubmitDate = DateTime.UtcNow
            };

            DisplayIDProvider.InjectDisplayId(entry);

            entry.Content = $"{entry.DisplayID}{Path.GetExtension(file.FileName)}";

            context.Entries.Add(entry);
            context.SaveChanges();

            string absoluteEntryPath = entry.GetAbsoluteUrl(environment);

            // Ensure that the directory exists
            Directory.CreateDirectory(Path.GetDirectoryName(absoluteEntryPath));

            using (var localFile = File.OpenWrite(absoluteEntryPath))
            using (var uploadedFile = file.OpenReadStream())
            {
                uploadedFile.CopyTo(localFile);
            }

            return entry;
        }

        /// <summary>
        /// Retrieves the submitted entry of the given user from the event, if possible.
        /// </summary>
        public Entry GetEntryOfUserInEvent(Event ev, UserAccount user)
            => ev.Entries.FirstOrDefault(x => x.Author.ID == user.ID);

        /// <summary>
        /// Deletes the specified entry.
        /// </summary>
        public bool DeleteEntry(Entry entry)
        {
            context.Entries.Remove(entry);
            return context.SaveChanges() > 0;
        }

        /// <summary>
        /// Creates a report for the given entry.
        /// </summary>
        public void ReportEntry(Entry entry, UserAccount reportAuthor, string reason)
        {
            Report report = new Report
            {
                Entry = entry,
                EntryAuthor = entry.Author,
                Event = entry.Event,
                ReportAuthor = reportAuthor,
                Reason = reason,
                Status = ReportStatus.Pending
            };

            context.Reports.Add(report);
            context.SaveChanges();
        }

        /// <summary>
        /// Updates the status of the given report.
        /// </summary>
        public void UpdateEntryReportStatus(Report report, ReportStatus status)
        {
            report.Status = status;
            context.SaveChanges();
        }
    }
}