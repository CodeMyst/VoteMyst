using System;
using System.Linq;

namespace VoteMyst.Database
{
    /// <summary>
    /// Provides utility to handle <see cref="Vote"/>s.
    /// </summary>
    public class VoteHelper
    {
        private readonly VoteMystContext _context;

        public VoteHelper(VoteMystContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Adds a vote from the specified user to the specified entry.
        /// </summary>
        public Vote AddVote(Entry entry, UserAccount user)
        {
            Vote vote = new Vote
            {
                Entry = entry,
                User = user,
                VoteDate = DateTime.UtcNow
            };

            _context.Votes.Add(vote);
            _context.SaveChanges();

            return vote;
        }

        /// <summary>
        /// Deletes the specified vote.
        /// </summary>
        public bool DeleteVote(Vote vote)
        {
            _context.Votes.Remove(vote);
            return _context.SaveChanges() > 0;
        }

        /// <summary>
        /// Returns the vote the user cast on the specified entry. Null if no vote exists.
        /// </summary>
        public Vote GetVoteByUserOnEntry(UserAccount user, Entry entry)
            => entry.Votes.FirstOrDefault(v => v.User.ID == user.ID);
    }

}