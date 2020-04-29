using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VoteMyst.Database.Models 
{
    public class UserData
    {
        [Key, Column("snowflake")]
        public ulong Snowflake { get; set; }
        [Column("first_seen")]
        public DateTime JoinDate { get; set; }
    }
}