namespace VoteMyst.Database 
{
    public static class AccountStateExtension
    {

        public static readonly AccountState[] ApplyableStates = { AccountState.Banned, AccountState.Active, AccountState.Moderator, AccountState.Admin };

        private const Permissions Banned = 0;
        private const Permissions Guest = Permissions.ViewEntries | Permissions.ViewEvents;
        private const Permissions Default = Guest | Permissions.SubmitEntries | Permissions.SubmitVotes | Permissions.EditEntries | Permissions.ViewUsers | Permissions.AllowWinning;
        private const Permissions Moderator = (Default | Permissions.DeleteEntries | Permissions.DeleteVotes) ^ Permissions.AllowWinning;
        private const Permissions Admin = (Permissions) ulong.MaxValue ^ Permissions.AllowWinning;

        public static Permissions GetDefaultPermissions(this AccountState accountState)
        {
            switch(accountState)
            {
                case AccountState.Admin:
                    return Admin;
                case AccountState.Moderator:
                    return Moderator;
                case AccountState.Banned:
                    return Banned;
                case AccountState.Guest:
                    return Guest;
                default:
                    return Default;
            }
        }
    }
}