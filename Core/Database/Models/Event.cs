using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VoteMyst.Database 
{
    /// <summary>
    /// Represents an event that contains entries posted by users.
    /// </summary>
    public class Event : IValidatableObject, IDatabaseEntity, IPublicDisplayable, ILinkable
    {
        private const string _dateTimeFormat = "{0:yyyy-MM-ddTHH:mm}";

        /// <summary>
        /// The primary database ID for the event.
        /// </summary>
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        /// <summary>
        /// The publically visible ID of the event.
        /// </summary>
        [Required, DisplayID(16), Column(TypeName = "VARCHAR(16)")]
        public string DisplayID { get; set; }

        /// <summary>
        /// The vanity URL of the event. Can be empty, in which case the DisplayID should use.
        /// </summary>
        [MinLength(4), StringLength(32)]
        [RegularExpression(@"^[a-zA-Z\d\-]*$", ErrorMessage = "The event URL may only contain lowercase letters, digits and dashes.")]
        public string URL { get; set; }

        /// <summary>
        /// The title of the event.
        /// </summary>
        [Required, StringLength(64)]
        public string Title { get; set; }

        /// <summary>
        /// The description of the event.
        /// </summary>
        [StringLength(2048)]
        public string Description { get; set; }

        /// <summary>
        /// The type of the event.
        /// </summary>
        [Required, Display(Name = "Type")]
        public EventType EventType { get; set; }

        /// <summary>
        /// The settings for the event.
        /// </summary>
        [Display(Name = "Settings")]
        public EventSettings Settings { get; set; } = EventSettings.Default;

        /// <summary>
        /// The UTC date when the event will be revealed to non-hosts.
        /// </summary>
        [Required, Display(Name = "Reveal Date"), DataType(DataType.DateTime)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = _dateTimeFormat)]
        public DateTime RevealDate { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// The UTC date when submissions for the event open.
        /// </summary>
        [Required, Display(Name = "Start Date"), DataType(DataType.DateTime)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = _dateTimeFormat)]
        public DateTime StartDate { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// The UTC date when submissions for the event should close and voting should open.
        /// </summary>
        [Required, Display(Name = "Submission End Date"), DataType(DataType.DateTime)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = _dateTimeFormat)]
        public DateTime SubmissionEndDate { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// The UTC date when voting for the event should end.
        /// </summary>
        [Required, Display(Name = "Vote End Date"), DataType(DataType.DateTime)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = _dateTimeFormat)]
        public DateTime VoteEndDate { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// The entries contained in this event.
        /// </summary>
        public virtual ICollection<Entry> Entries { get; set; }
        /// <summary>
        /// The reports contained in this event.
        /// </summary>
        public virtual ICollection<Report> Reports { get; set; }

        public Event()
        {
            Entries = new HashSet<Entry>();
        }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (RevealDate >= StartDate || StartDate >= SubmissionEndDate || SubmissionEndDate >= VoteEndDate)
            {
                yield return new ValidationResult("All dates must be in ascending order.");
            }

            var provider = validationContext.GetService(typeof(DatabaseHelperProvider)) as DatabaseHelperProvider;

            Event conflictingEvent = provider.Events.GetEventByUrl(URL);
            if (conflictingEvent != null && conflictingEvent.ID != ID)
            {
                // Users could potentially detect hidden events by repeatedly trying to create events here.
                yield return new ValidationResult("That URL is already in use.");
            }
        }

        public EventState GetStateForTime(DateTime dateTime)
        {
            if (dateTime < RevealDate)
                return EventState.Hidden;
            if (dateTime < StartDate)
                return EventState.Revealed;
            if (dateTime < SubmissionEndDate)
                return EventState.Ongoing;
            if (dateTime < VoteEndDate)
                return EventState.Voting;
            
            return EventState.Closed;
        }
        public EventState GetCurrentState()
            => GetStateForTime(DateTime.UtcNow);

        public string GetUrl()
            => $"/events/{GetValidDisplayUrl()}";

        public string GetValidDisplayUrl()
            => URL ?? DisplayID;

        public override string ToString()
            => $"Event('{DisplayID}', {Title})";
    }
}