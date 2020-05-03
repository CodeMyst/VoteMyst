namespace VoteMyst.Database
{
    public class DatabaseHelperProvider
    {
        public AuthorizationHelper Authorization { get; }
        public EntryHelper Entries { get; }
        public EventHelper Events { get; }
        public UserDataHelper Users { get; }
        public VoteHelper Votes { get; }

        public DatabaseHelperProvider(VoteMystContext context)
        {
            Users = new UserDataHelper(context);
            Authorization = new AuthorizationHelper(context, Users);
            Entries = new EntryHelper(context);
            Events = new EventHelper(context);
            Votes = new VoteHelper(context);
        }
    }
}