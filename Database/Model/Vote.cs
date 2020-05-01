using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VoteMyst.Database.Models 
{
    public class Vote
    {
        [Key, Column("vote_id"), DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int VoteId { get; set; }
        [Column("snowflake")]
        public ulong Snowflake { get; set; }
        [Column("entry_id")]
        public int EntryId { get; set; }
        [Column("vote_date")]
        public DateTime VoteDate { get; set; }
    }
}