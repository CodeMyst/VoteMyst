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
        [Column("snowflake")]
        public ulong Snowflake { get; set; }
        [Column("submit_date")]
        public DateTime SubmitDate { get; set; }
        [Column("entry_type")]
        public EntryType EntryType { get; set; }
        [Column("entry")]
        public string Content { get; set; }
    }
}