using System;
using System.Linq;
using VoteMyst.Database.Models;

namespace VoteMyst.Database
{
    public class DatabaseHelper
    {
        private static readonly VoteMystContext context = VoteMystContext.Context;

        public static class UserDataHelper
        {
            public static bool NewUser(ulong snowflake, out UserData user)
            {
                user = new UserData() {
                    Snowflake = snowflake,
                    JoinDate = DateTime.UtcNow,
                    PermissionLevel = Permissions.Admin
                };

                context.UserData.Add(user);

                return context.SaveChanges() > 0;
            }

            public static UserData GetOrCreateUser(ulong snowflake)
            {
                var userData = context.UserData.FirstOrDefault(x => x.Snowflake == snowflake);

                if (userData == null && !NewUser(snowflake, out userData))
                    userData = null;

                return userData;
            }

            public static bool DeleteUser(ulong snowflake)
            => DeleteUser(GetOrCreateUser(snowflake));

            public static bool DeleteUser(UserData user)
            {
                context.UserData.Remove(user);

                return context.SaveChanges() > 0;
            }

            public static bool AddPermission(ulong snowflake, Permissions permissions) 
            {
                UserData user = GetOrCreateUser(snowflake);
            
                user.PermissionLevel |= permissions;

                context.UserData.Update(user);

                return context.SaveChanges() > 0;
            }

            public static bool RemovePermission(ulong snowflake, Permissions permissions) 
            {
                UserData user = GetOrCreateUser(snowflake);
            
                user.PermissionLevel ^= permissions;

                context.UserData.Update(user);

                return context.SaveChanges() > 0;
            }
        }

        public static class EventHelper
        {
            public static bool CreateEvent(string name, string description, 
                                           DateTime startDate, DateTime endDate, 
                                           DateTime voteEndDate, out Event ev) 
            {
                ev = new Event() {
                    Title = name,
                    Description = description,
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

        public static class EntryHelper
        {
            public static bool CreateEntry(Event ev, UserData user, EntryType type, string content, out Entry entry)
            => CreateEntry(ev.EventId, user.Snowflake, type, content, out entry);

            public static bool CreateEntry(int eventId, ulong snowflake, EntryType type, string content, out Entry entry)
            {
                entry = new Entry() {
                    EventId = eventId,
                    Snowflake = snowflake,
                    EntryType = type,
                    Content = content,
                    SubmitDate = DateTime.UtcNow
                };

                context.Entries.Add(entry);

                return context.SaveChanges() > 0;
            }

            public static Entry GetEntry(int entryID)
            => context.Entries
                .FirstOrDefault(x => x.EntryId == entryID);
            
            public static Entry[] GetEntriesFromUser(UserData user)
            => GetEntriesFromUser(user.Snowflake);

            public static Entry[] GetEntriesFromUser(ulong snowflake)
            => context.Entries
                .Where(x => x.Snowflake == snowflake)
                .ToArray();

            public static Entry[] GetEntriesInEvent(Event ev) 
            => GetEntriesInEvent(ev.EventId);

            public static Entry[] GetEntriesInEvent(int eventId)
            => context.Entries
                .Where(x => x.EventId == eventId)
                .ToArray();

            public static Entry GetEntryOfUserInEvent(Event ev, UserData user)
            => GetEntryOfUserInEvent(ev.EventId, user.Snowflake);

            public static Entry GetEntryOfUserInEvent(int eventId, ulong snowflake)
            => context.Entries
                .Where(x => x.Snowflake == snowflake)
                .FirstOrDefault(x => x.EventId == eventId);
        }

        public static class VoteHelper
        {
            public static bool AddVote(Entry entry, UserData user, out Vote vote) 
            => AddVote(entry.EntryId, user.Snowflake, out vote);

            public static bool AddVote(int entryId, ulong snowflake, out Vote vote) 
            {
                vote = new Vote() {
                    EntryId = entryId,
                    Snowflake = snowflake,
                    VoteDate = DateTime.UtcNow
                };

                context.Votes.Add(vote);

                return context.SaveChanges() > 0;
            }

            public static bool DeleteVote(Vote vote)
            => DeleteVote(vote.VoteId);

            public static bool DeleteVote(int voteId) 
            {
                context.Votes
                    .Remove(GetVote(voteId));

                return context.SaveChanges() > 0;
            }

            public static bool DeleteVote(Entry entry, UserData user)
            => DeleteVote(entry.EntryId, user.Snowflake);

            public static bool DeleteVote(int entryId, ulong snowflake) 
            {
                context.Votes
                    .Remove(GetVoteByUserOnEntry(snowflake, entryId));

                return context.SaveChanges() > 0;
            }

            public static Vote GetVote(int voteId) 
            => context.Votes
                .FirstOrDefault(x => x.VoteId == voteId);

            public static Vote GetVoteByUserOnEntry(UserData user, Entry entry)
            => GetVoteByUserOnEntry(user.Snowflake, entry.EntryId);

            public static Vote GetVoteByUserOnEntry(ulong snowflake, int entryId)
            => context.Votes
                .Where(x => x.EntryId == entryId)
                .FirstOrDefault(x => x.Snowflake == snowflake);

            public static Vote[] GetAllVotesOfUser(UserData user)
            => GetAllVotesOfUser(user.Snowflake);
            
            public static Vote[] GetAllVotesOfUser(ulong snowflake)
            => context.Votes
                .Where(x => x.Snowflake == snowflake)
                .ToArray();

            public static Vote[] GetAllVotesForEntry(Entry entry)
            => GetAllVotesForEntry(entry.EntryId);

            public static Vote[] GetAllVotesForEntry(int entryId)
            => context.Votes
                .Where(x => x.EntryId == entryId)
                .ToArray();

            public static Vote[] GetAllVotesInEvent(Event ev)
            => GetAllVotesInEvent(ev.EventId);

            public static Vote[] GetAllVotesInEvent(int eventId)
            => context.Entries
                .Where(x => x.EventId == eventId)
                .Join(context.Votes,
                    entry => entry.EntryId,
                    vote => vote.EntryId,
                    (entry, vote) => vote)
                .ToArray();
        }
    }
}