namespace VoteMyst.Database
{
    public class DatabaseHelperProvider
    {
        public VoteMystContext Context { get; }

        public AuthorizationHelper Authorization { get; }
        public EntryHelper Entries { get; }
        public EventHelper Events { get; }
        public UserAccountHelper Users { get; }
        public VoteHelper Votes { get; }

        public DatabaseHelperProvider(VoteMystContext context, AvatarHelper avatarHelper)
        {
            Context = context;

            Authorization = new AuthorizationHelper(context);
            Users = new UserAccountHelper(context, avatarHelper);
            Entries = new EntryHelper(context);
            Events = new EventHelper(context);
            Votes = new VoteHelper(context);
        }
    }
}