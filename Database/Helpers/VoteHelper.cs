using System;
using System.Linq;
using VoteMyst.Database.Models;

namespace VoteMyst.Database
{
    public static class VoteHelper
    {
        private static readonly VoteMystContext context = VoteMystContext.Context;

        public static bool AddVote(Entry entry, UserData user, out Vote vote)
            => AddVote(entry.EntryId, user.Snowflake, out vote);

        public static bool AddVote(int entryId, ulong snowflake, out Vote vote)
        {
            vote = new Vote()
            {
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