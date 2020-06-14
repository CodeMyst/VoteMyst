using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VoteMyst.Database 
{
    /// <summary>
    /// Represents an entry that was posted inside of an event by a user.
    /// </summary>
    public class Entry : IDatabaseEntity, IPublicDisplayable, ILinkable
    {
        /// <summary>
        /// The primary database ID for the entry.
        /// </summary>
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        /// <summary>
        /// The ID used for publically referring to the entry inside of an event.
        /// </summary>
        [Required, DisplayID(16), Column(TypeName = "VARCHAR(16)")]
        public string DisplayID { get; set; }

        /// <summary>
        /// The <see cref="VoteMyst.Database.Event" /> the entry resides in.
        /// </summary>
        [Required]
        public virtual Event Event { get; set; }

        /// <summary>
        /// The <see cref="VoteMyst.Database.UserAccount" /> the entry was posted by.
        /// </summary>
        [Required]
        public virtual UserAccount Author { get; set; }

        /// <summary>
        /// The UTC date the entry was posted at.
        /// </summary>
        [Required]
        public DateTime SubmitDate { get; set; }

        /// <summary>
        /// The type of the entry.
        /// </summary>
        [Required]
        public EntryType EntryType { get; set; }

        /// <summary>
        /// The content of the entry. This should be interpreted according to a combination of the EntryType and the EventType.
        /// </summary>
        [Required]
        public string Content { get; set; }

        public virtual ICollection<Vote> Votes { get; set; }

        public virtual ICollection<Report> Reports { get; set; }

        public Entry()
        {
            Votes = new HashSet<Vote>();
        }

        public string GetUrl()
            => $"{Event.GetUrl()}#{DisplayID}";

        public override string ToString()
            => $"Entry('{DisplayID}', {EntryType}, {Author}@{Event})";
    }
}