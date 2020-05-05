using VoteMyst.Database.Models;

namespace VoteMyst.Database
{

    public static class UserDataExtensions
    {
        public static bool HasPermission(this UserData user, Permissions permissions) 
            => user.PermissionLevel.HasFlag(permissions);

        public static bool IsBanned(this UserData user)
            => user.AccountState == AccountState.Banned;
            
        public static bool IsDeleted(this UserData user)
            => user.AccountState == AccountState.Deleted;

        public static bool IsModerator(this UserData user)
            => user.AccountState == AccountState.Moderator;

        public static bool IsAdmin(this UserData user)
            => user.AccountState == AccountState.Admin;

        public static string GetAccountState(this UserData user)
            =>  user.AccountState.ToString();
    }    

}