using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VoteMyst.Database
{
    /// <summary>
    /// Represents a modification of a users event permissions.
    /// </summary>
    public class EventPermissionModifier : IDatabaseEntity
    {
        /// <summary>
        /// The primary database ID for the event permissions.
        /// </summary>
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        /// <summary>
        /// The <see cref="VoteMyst.Database.UserAccount"/> that the permissions should apply to.
        /// </summary>
        [Required]
        public virtual UserAccount User { get; set; }

        /// <summary>
        /// The <see cref="VoteMyst.Database.Event"/> the permissions should apply to.
        /// </summary>
        [Required]
        public virtual Event Event { get; set; }

        /// <summary>
        /// The permissions that should be applied to the user inside the event.
        /// </summary>
        public EventPermissions Permissions { get; set; } = EventPermissions.Default;
    }
}