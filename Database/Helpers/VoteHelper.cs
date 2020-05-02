using System;
using System.Linq;
using VoteMyst.Database.Models;

namespace VoteMyst.Database
{
    public class VoteHelper
    {
        private readonly VoteMystContext context;

        public VoteHelper(VoteMystContext context)
        {
            this.context = context;
        }

        public Vote AddVote(Entry entry, UserData user)
            => AddVote(entry.EntryId, user.UserId);

        public Vote AddVote(int entryId, int userId)
        {
            Vote vote = new Vote()
            {
                EntryId = entryId,
                UserId = userId,
                VoteDate = DateTime.UtcNow
            };

            context.Votes.Add(vote);

            context.SaveChanges();

            return vote;
        }

        public bool DeleteVote(Vote vote)
            => DeleteVote(vote.VoteId);

        public bool DeleteVote(int voteId)
        {
            context.Votes
                .Remove(GetVote(voteId));

            return context.SaveChanges() > 0;
        }

        public bool DeleteVote(Entry entry, UserData user)
            => DeleteVote(entry.EntryId, user.UserId);

        public bool DeleteVote(int entryId, int userId)
        {
            context.Votes
                .Remove(GetVoteByUserOnEntry(userId, entryId));

            return context.SaveChanges() > 0;
        }

        public Vote GetVote(int voteId)
            => context.Votes
                .FirstOrDefault(x => x.VoteId == voteId);

        public Vote GetVoteByUserOnEntry(UserData user, Entry entry)
            => GetVoteByUserOnEntry(user.UserId, entry.EntryId);

        public Vote GetVoteByUserOnEntry(int userId, int entryId)
            => context.Votes
                .Where(x => x.EntryId == entryId)
                .FirstOrDefault(x => x.UserId == userId);

        public Vote[] GetAllVotesOfUser(UserData user)
            => GetAllVotesOfUser(user.UserId);

        public Vote[] GetAllVotesOfUser(int userId)
            => context.Votes
                .Where(x => x.UserId == userId)
                .ToArray();

        public Vote[] GetAllVotesForEntry(Entry entry)
            => GetAllVotesForEntry(entry.EntryId);

        public Vote[] GetAllVotesForEntry(int entryId)
            => context.Votes
                .Where(x => x.EntryId == entryId)
                .ToArray();

        public Vote[] GetAllVotesInEvent(Event ev)
            => GetAllVotesInEvent(ev.EventId);

        public Vote[] GetAllVotesInEvent(int eventId)
            => context.Entries
                .Where(x => x.EventId == eventId)
                .Join(context.Votes,
                    entry => entry.EntryId,
                    vote => vote.EntryId,
                    (entry, vote) => vote)
                .ToArray();
    }

}