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

        public bool CreateEvent(string name, string description,
                                       EventType eventType, DateTime revealDate,
                                       DateTime startDate, DateTime endDate,
                                       DateTime voteEndDate, out Event ev)
        {
            ev = new Event()
            {
                Title = name,
                Description = description,
                EventType = eventType,
                RevealDate = revealDate,
                StartDate = startDate,
                EndDate = endDate,
                VoteEndDate = voteEndDate
            };

            context.Events.Add(ev);

            return context.SaveChanges() > 0;
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

        public Event[] GetEventsUserParticipatedIn(ulong snowflake)
            => context.Entries
                .Where(x => x.Snowflake == snowflake)
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
    }

}