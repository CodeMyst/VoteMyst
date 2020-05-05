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

        public Entry CreateEntry(Event ev, UserData user, EntryType type, string content)
            => CreateEntry(ev.EventId, user.UserId, type, content);

        public Entry CreateEntry(int eventId, int userId, EntryType type, string content)
        {
            Entry entry = new Entry()
            {
                EventId = eventId,
                UserId = userId,
                EntryType = type,
                Content = content,
                SubmitDate = DateTime.UtcNow
            };

            context.Entries.Add(entry);

             context.SaveChanges();

             return entry;
        }

        public Entry GetEntry(int entryID)
            => context.Entries
                .FirstOrDefault(x => x.EntryId == entryID);

        public Entry[] GetEntriesFromUser(UserData user)
            => GetEntriesFromUser(user.UserId);

        public Entry[] GetEntriesFromUser(int userId)
            => context.Entries
                .Where(x => x.UserId == userId)
                .ToArray();

        public Entry[] GetEntriesInEvent(Event ev)
            => GetEntriesInEvent(ev.EventId);

        public Entry[] GetEntriesInEvent(int eventId)
            => context.Entries
                .Where(x => x.EventId == eventId)
                .ToArray();

        public Entry GetEntryOfUserInEvent(Event ev, UserData user)
            => GetEntryOfUserInEvent(ev.EventId, user.UserId);

        public Entry GetEntryOfUserInEvent(int eventId, int userId)
            => context.Entries
                .Where(x => x.UserId == userId)
                .FirstOrDefault(x => x.EventId == eventId);

        public bool DeleteEntry(Entry entry)
        {
            context.Entries.Remove(entry);
            return context.SaveChanges() > 0;
        }
    }

}