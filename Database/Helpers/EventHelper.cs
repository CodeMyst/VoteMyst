using System;
using System.Linq;
using VoteMyst.Database.Models;

namespace VoteMyst.Database
{
    public class EventHelper
    {
        private readonly VoteMystContext context;

        public EventHelper(VoteMystContext context)
        {
            this.context = context;
        }

        public Event CreateEvent(string name, string description,
                                       EventType eventType, DateTime revealDate,
                                       DateTime startDate, DateTime endDate,
                                       DateTime voteEndDate)
        {
            Event ev = new Event()
            {
                Title = name,
                Description = description,
                EventType = eventType,
                RevealDate = revealDate,
                StartDate = startDate,
                EndDate = endDate,
                VoteEndDate = voteEndDate
            };
            
            return CreateEvent(ev);
        }
        public Event CreateEvent(Event ev)
        {
            context.Events.Add(ev);
            context.SaveChanges();
            return ev;
        }

        public Event GetEvent(int eventId)
            => context.Events
                .FirstOrDefault(x => x.EventId == eventId);

        public Event[] GetEventLikeName(string name)
            => context.Events
                .Where(x => x.Title.Contains(name, StringComparison.InvariantCultureIgnoreCase))
                .ToArray();

        public Event[] GetEventsBetween(DateTime start, DateTime end)
            => context.Events
                .Where(x => x.StartDate >= start)
                .Where(x => x.StartDate <= end)
                .ToArray();

        public Event[] GetEventsUserParticipatedIn(int userId)
            => context.Entries
                .Where(x => x.UserId == userId)
                .Join(context.Events.AsEnumerable(),
                    x => x.EventId,
                    y => y.EventId,
                    (x, y) => y)
                .ToArray();

        public Event[] GetAllEventsAfter(DateTime date)
            => context.Events
                .Where(x => x.StartDate >= date)
                .ToArray();

        public Event[] GetAllEventsBefore(DateTime date)
            => context.Events
                .Where(x => x.StartDate <= date)
                .ToArray();

        public Event[] GetAllEventsFinishedBefore(DateTime date)
            => context.Events
                .Where(x => x.VoteEndDate < date)
                .OrderByDescending(x => x.StartDate)
                .ToArray();

        public Event[] GetCurrentEvents()
            => context.Events
                .Where(x => x.StartDate <= DateTime.UtcNow)
                .Where(x => x.VoteEndDate > DateTime.UtcNow)
                .OrderBy(x => x.VoteEndDate)
                .ToArray();
        
        public Event[] GetPlannedEvents(bool ignoreRevealDate = false)
            => context.Events
                .Where(x => x.StartDate > DateTime.UtcNow)
                .Where(x => ignoreRevealDate || x.RevealDate <= DateTime.UtcNow)
                .OrderBy(x => x.StartDate)
                .ToArray();
    }

}