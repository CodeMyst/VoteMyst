namespace VoteMyst.Controllers.Api.Models
{
    /// <summary>
    /// Represents the result of a voting action.
    /// </summary>
    public class VoteResult
    {
        /// <summary>
        /// Indicates if the voting action was successful.
        /// </summary>
        public bool ActionSuccess { get; set; }
        /// <summary>
        /// Indicates if the entry has a vote after the action has performed.
        /// </summary>
        public bool HasVote { get; set; }

        public VoteResult(bool success, bool hasVote)
        {
            ActionSuccess = success;
            HasVote = hasVote;
        }
    }
}
