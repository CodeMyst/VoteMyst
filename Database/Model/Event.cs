using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VoteMyst.Database.Models 
{
    public class Event
    {
        [Key, Column("event_id"), DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int EventId { get; set; }
        [Column("start_date")]
        public DateTime StartDate { get; set; }
        [Column("end_date")]
        public DateTime EndDate { get; set; }
        [Column("vote_end_date")]
        public DateTime VoteEndDate { get; set; }
        [Column("name")]
        public string Title { get; set; }
        [Column("description")]
        public string Description { get; set; }
    }
}