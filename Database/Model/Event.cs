using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VoteMyst.Database.Models 
{
    public class Event : IValidatableObject
    {
        private const string _dateTimeFormat = "{0:yyyy-MM-ddTHH:mm}";

        [Key, Column("event_id"), DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int EventId { get; set; }

        [Column("name")]
        [Required]
        [StringLength(64)]
        public string Title { get; set; }

        [Column("description")]
        [StringLength(512)]
        public string Description { get; set; }

        [Column("event_type")]
        [Display(Name = "Event Type")]
        public EventType EventType { get; set; }

        [Column("reveal_date")]
        [Display(Name = "Reveal Date")]
        [Required]
        [DataType(DataType.DateTime)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = _dateTimeFormat)]
        public DateTime RevealDate { get; set; } = DateTime.UtcNow;

        [Column("start_date")]
        [Display(Name = "Start Date")]
        [Required]
        [DataType(DataType.DateTime)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = _dateTimeFormat)]
        public DateTime StartDate { get; set; } = DateTime.UtcNow;

        [Column("end_date")]
        [Display(Name = "End Date")]
        [Required]
        [DataType(DataType.DateTime)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = _dateTimeFormat)]
        public DateTime EndDate { get; set; } = DateTime.UtcNow;

        [Column("vote_end_date")]
        [Display(Name = "Vote End Date")]
        [Required]
        [DataType(DataType.DateTime)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = _dateTimeFormat)]
        public DateTime VoteEndDate { get; set; } = DateTime.UtcNow;

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (RevealDate >= StartDate || StartDate >= EndDate || EndDate >= VoteEndDate)
            {
                yield return new ValidationResult("All dates must be in ascending order.");
            }
        }
    }
}