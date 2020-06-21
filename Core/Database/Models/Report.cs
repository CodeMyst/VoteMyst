using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VoteMyst.Database
{
    /// <summary>
    /// Represents a report on a post.
    /// </summary>
    public class Report : IDatabaseEntity
    {
        /// <summary>
        /// The primary database ID for the report.
        /// </summary>
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        /// <summary>
        /// The <see cref="UserAccount"/> that submit the report.
        /// </summary>
        public virtual UserAccount ReportAuthor { get; set; }

        /// <summary>
        /// The <see cref="UserAccount"/> that submitted the reported <see cref="Database.Entry"/>.
        /// </summary>
        public virtual UserAccount EntryAuthor { get; set; }

        /// <summary>
        /// The <see cref="Database.Event"/> that the reported entry is contained in.
        /// </summary>
        public virtual Event Event { get; set; }

        /// <summary>
        /// The <see cref="Database.Entry"/> that was reported.
        /// </summary>
        public virtual Entry Entry { get; set; }

        /// <summary>
        /// The reason why the post was reported.
        /// </summary>
        public string Reason { get; set; }

        /// <summary>
        /// The current state of the report.
        /// </summary>
        public ReportStatus Status { get; set; }

        public override string ToString()
            => $"Report({ReportAuthor}->{Entry})";
    }
}