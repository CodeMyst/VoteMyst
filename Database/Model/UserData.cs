using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VoteMyst.Database.Models 
{
    public class UserData
    {
        [Key, Column("user_id"), DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int UserId { get; set; }
        [Column("display_id")]
        public string DisplayId { get; set; }
        [Column("first_seen")]
        public DateTime JoinDate { get; set; }
        [Column("permissions")]
        public Permissions PermissionLevel { get; set; }
        [Column("account_state")]
        public AccountState AccountState { get; set; }
        [Column("username")]
        public string Username { get; set; }
    }
}