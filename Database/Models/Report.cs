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
        /// The <see cref="VoteMyst.Database.UserAccount"/> that submit the report.
        /// </summary>
        [Required]
        public virtual UserAccount User { get; set; }

        /// <summary>
        /// The <see cref="VoteMyst.Database.Entry"/> that was reported.
        /// </summary>
        [Required]
        public virtual Entry Entry { get; set; }

        /// <summary>
        /// The reason why the post was reported.
        /// </summary>
        public string Reason { get; set; }

        public override string ToString()
            => $"Report({User}->{Entry})";
    }
}