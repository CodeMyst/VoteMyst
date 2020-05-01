using VoteMyst.Database.Models;

namespace VoteMyst.Database
{

    public static class UserDataExtensions
    {
        public static bool HasPermission(this UserData user, Permissions permissions) 
            => user.PermissionLevel.HasFlag(permissions);

    }    

}