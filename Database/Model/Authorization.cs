using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VoteMyst.Database.Models
{
    public class Authorization
    {
        [Key, Column("auth_id"), DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int AuthId { get; set; }
        [Column("user_id")]
        public int UserId { get; set; }
        [Column("service_id")]
        public string ServiceUserId { get; set; }
        [Column("service_type")]
        public ServiceType ServiceType { get; set; }
    }
}