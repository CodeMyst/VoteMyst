using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VoteMyst.Database
{
    /// <summary>
    /// Represents a user account on the service.
    /// </summary>
    public class UserAccount : IDatabaseEntity, IPublicDisplayable, ILinkable
    {
        /// <summary>
        /// The primary database ID for the user account.
        /// </summary>
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        /// <summary>
        /// The ID used for publically referring to the user account.
        /// </summary>
        [Required, DisplayID(28), Column(TypeName = "VARCHAR(28)")]
        public string DisplayID { get; set; }

        /// <summary>
        /// The username of the account.
        /// </summary>
        [Required]
        public string Username { get; set; }

        /// <summary>
        /// The date that the user first joined the site.
        /// </summary>
        [Required]
        public DateTime JoinDate { get; set; }

        /// <summary>
        /// The permission level of the user account.
        /// </summary>
        public GlobalPermissions Permissions { get; set; }

        /// <summary>
        /// The account badge for the account.
        /// </summary>
        public AccountBadge AccountBadge { get; set; }

        public virtual ICollection<Authorization> Authorizations { get; set; }
        public virtual ICollection<Entry> Entries { get; set; }
        public virtual ICollection<Report> AuthoredReports { get; set; }
        public virtual ICollection<Vote> AuthoredVotes { get; set; }

        public UserAccount()
        {
            Authorizations = new HashSet<Authorization>();
            Entries = new HashSet<Entry>();
            AuthoredReports = new HashSet<Report>();
            AuthoredVotes = new HashSet<Vote>();
        }

        public string GetUrl()
            => $"/users/display/{DisplayID}";

        public override string ToString()
            => $"UserAccount('{DisplayID}', {Username})";
    }
}