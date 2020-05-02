using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VoteMyst.Database.Models 
{
    public class Entry
    {
        [Key, Column("entry_id"), DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int EntryId { get; set; }
        [Column("event_id")]
        public int EventId { get; set; }
        [Column("user_id")]
        public int UserId { get; set; }
        [Column("submit_date")]
        public DateTime SubmitDate { get; set; }
        [Column("entry_type")]
        public EntryType EntryType { get; set; }
        [Column("entry")]
        public string Content { get; set; }
    }
}