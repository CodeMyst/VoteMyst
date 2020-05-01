using System;
using System.Linq;
using VoteMyst.Database.Models;

namespace VoteMyst.Database
{
    public class EntryHelper
    {

        private readonly VoteMystContext context;

        public EntryHelper(VoteMystContext context)
        {
            this.context = context;
        }

        public bool CreateEntry(Event ev, UserData user, EntryType type, string content, out Entry entry)
            => CreateEntry(ev.EventId, user.Snowflake, type, content, out entry);

        public bool CreateEntry(int eventId, ulong snowflake, EntryType type, string content, out Entry entry)
        {
            entry = new Entry()
            {
                EventId = eventId,
                Snowflake = snowflake,
                EntryType = type,
                Content = content,
                SubmitDate = DateTime.UtcNow
            };

            context.Entries.Add(entry);

            return context.SaveChanges() > 0;
        }

        public Entry GetEntry(int entryID)
            => context.Entries
                .FirstOrDefault(x => x.EntryId == entryID);

        public Entry[] GetEntriesFromUser(UserData user)
            => GetEntriesFromUser(user.Snowflake);

        public Entry[] GetEntriesFromUser(ulong snowflake)
            => context.Entries
                .Where(x => x.Snowflake == snowflake)
                .ToArray();

        public Entry[] GetEntriesInEvent(Event ev)
            => GetEntriesInEvent(ev.EventId);

        public Entry[] GetEntriesInEvent(int eventId)
            => context.Entries
                .Where(x => x.EventId == eventId)
                .ToArray();

        public Entry GetEntryOfUserInEvent(Event ev, UserData user)
            => GetEntryOfUserInEvent(ev.EventId, user.Snowflake);

        public Entry GetEntryOfUserInEvent(int eventId, ulong snowflake)
            => context.Entries
                .Where(x => x.Snowflake == snowflake)
                .FirstOrDefault(x => x.EventId == eventId);
    }

}