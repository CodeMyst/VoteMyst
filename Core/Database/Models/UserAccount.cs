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
        [Required(ErrorMessage = "The username may not be empty.")]
        [MinLength(2, ErrorMessage = "The username must be atleast 2 characters long."), MaxLength(32, ErrorMessage = "The username may not be longer than 32 characters.")]
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
        [Display(Name = "Badge")]
        public AccountBadge AccountBadge { get; set; }

        /// <summary>
        /// The <see cref="Authorization"/>s associated with this account.
        /// </summary>
        public virtual ICollection<Authorization> Authorizations { get; set; }
        /// <summary>
        /// The entries submitted by this account.
        /// </summary>
        public virtual ICollection<Entry> Entries { get; set; }
        /// <summary>
        /// The votes authored by this account.
        /// </summary>
        public virtual ICollection<Vote> AuthoredVotes { get; set; }

        public UserAccount()
        {
            Authorizations = new HashSet<Authorization>();
            Entries = new HashSet<Entry>();
            AuthoredVotes = new HashSet<Vote>();
        }

        public string GetUrl()
            => $"/users/{DisplayID}";

        public override string ToString()
            => $"UserAccount('{DisplayID}', {Username})";
    }
}