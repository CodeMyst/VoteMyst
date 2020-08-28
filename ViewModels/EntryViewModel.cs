using System.Linq;

using VoteMyst.Database;

namespace VoteMyst.ViewModels
{
    public class EntryViewModel
    {
        public Entry Entry { get; }
        
        public UserAccount Author => Entry.Author;
        public Event Event => Entry.Event;

        public EventPermissions EventPermissions { get; }
        public EventState EventState { get; }

        public bool VotingOpen => EventState == EventState.Voting;
        public bool DisplayUser => EventState == EventState.Closed;
        
        public bool IsSelfPost { get; }
        public bool HasVoted { get; }
        public bool CanVote { get; }
        public bool CanReport { get; }
        public bool CanDelete { get; }

        public EntryViewModel(Entry entry, UserAccount currentUser, DatabaseHelperProvider databaseHelpers)
        {
            bool isGuest = currentUser.ID < 0;

            Entry = entry;

            EventPermissions = databaseHelpers.Events.GetUserPermissionsForEvent(currentUser, Event);
            EventState = Event.GetCurrentState();

            IsSelfPost = currentUser.ID == Author.ID;
            HasVoted = databaseHelpers.Votes.GetVoteByUserOnEntry(currentUser, entry) != null;

            CanVote = VotingOpen && !IsSelfPost && !isGuest;
            CanReport = !IsSelfPost && !isGuest 
                && !entry.Reports.Any(r => r.ReportAuthor.ID == currentUser.ID);
            CanDelete = IsSelfPost 
                || EventPermissions.HasFlag(EventPermissions.ManageEntries) 
                || currentUser.Permissions.HasFlag(GlobalPermissions.ManageAllEvents);
        }
    }
}