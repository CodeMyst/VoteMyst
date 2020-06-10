using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VoteMyst.Database
{
    /// <summary>
    /// Represents a method of authorization for a user account.
    /// </summary>
    public class Authorization : IDatabaseEntity
    {
        /// <summary>
        /// The primary database ID of the authorization entry.
        /// </summary>
        /// <value></value>
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        /// <summary>
        /// The ID of the user account linked to this authorization method.
        /// </summary>
        [Required]
        public virtual UserAccount User { get; set; }

        /// <summary>
        /// The hashed user ID on the specified service.
        /// </summary>
        [Required, Column(TypeName = "VARCHAR(64)")]
        public string ServiceUserID { get; set; }

        /// <summary>
        /// The service specified for this authorization method.
        /// </summary>
        [Required]
        public Service Service { get; set; }

        /// <summary>
        /// Is the authorization method still valid or has the linked account been disabled?
        /// </summary>
        [Required]
        public bool Valid { get; set; }

        public override string ToString()
            => $"{nameof(Authorization)}({ServiceUserID}({Service})->{User})";
    }
}