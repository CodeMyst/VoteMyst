using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VoteMyst.Database
{
    /// <summary>
    /// Represents a vote on an entry inside of an event.
    /// </summary>
    public class Vote : IDatabaseEntity
    {
        /// <summary>
        /// The primary database ID for the vote.
        /// </summary>
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        /// <summary>
        /// The <see cref="VoteMyst.Database.UserAccount"/> that cast the vote.
        /// </summary>
        [Required]
        public virtual UserAccount User { get; set; }

        /// <summary>
        /// The <see cref="VoteMyst.Database.Entry"/> the vote was cast on.
        /// </summary>
        [Required]
        public virtual Entry Entry { get; set; }

        /// <summary>
        /// The date that the vote was cast on.
        /// </summary>
        [Required]
        public DateTime VoteDate { get; set; }

        public override string ToString()
            => $"Vote({User}->{Entry})";
    }
}