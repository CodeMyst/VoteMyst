using System;
using System.Linq;
using VoteMyst.Database.Models;

namespace VoteMyst.Database
{
    public static class EventHelper
    {
        private static readonly VoteMystContext context = VoteMystContext.Context;

        public static bool CreateEvent(string name, string description,
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

        public static Event GetEvent(int eventId)
            => context.Events
                .FirstOrDefault(x => x.EventId == eventId);

        public static Event[] GetEventLikeName(string name)
            => context.Events
                .Where(x => x.Title.Contains(name, StringComparison.InvariantCultureIgnoreCase))
                .ToArray();

        public static Event[] GetEventsBetween(DateTime start, DateTime end)
            => context.Events
                .Where(x => x.StartDate >= start)
                .Where(x => x.StartDate <= end)
                .ToArray();

        public static Event[] GetEventsUserParticipatedIn(ulong snowflake)
            => context.Entries
                .Where(x => x.Snowflake == snowflake)
                .Join(context.Events.AsEnumerable(),
                    x => x.EventId,
                    y => y.EventId,
                    (x, y) => y)
                .ToArray();

        public static Event[] GetAllEventsAfter(DateTime date)
            => context.Events
                .Where(x => x.StartDate >= date)
                .ToArray();

        public static Event[] GetAllEventsBefore(DateTime date)
            => context.Events
                .Where(x => x.StartDate <= date)
                .ToArray();
    }

}