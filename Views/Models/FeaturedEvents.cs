using System;
using System.Linq;
using VoteMyst.Database;

namespace VoteMyst 
{
    public class FeaturedEvents
    {
        public Event[] TopOngoingEvents { get; set; } = new Event[0];
        public Event[] RecentUpcomingEvents { get; set; } = new Event[0];

        private const int _defaultEventCount = 2;

        public static FeaturedEvents Generate(DatabaseHelperProvider provider, int count = _defaultEventCount)
        {
            return new FeaturedEvents
            {
                TopOngoingEvents = provider.Events.GetCurrentEvents()
                    .OrderByDescending(e => e.Entries.Count)
                    .Take(count)
                    .ToArray(),

                RecentUpcomingEvents = provider.Events.GetPlannedEvents()
                    .OrderBy(e => DateTime.UtcNow - e.StartDate)
                    .Take(count)
                    .ToArray()
            };
        }
    }
}