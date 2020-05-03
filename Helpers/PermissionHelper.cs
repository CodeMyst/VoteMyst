using VoteMyst.Database;
using VoteMyst.Database.Models;

namespace VoteMyst 
{
    public static class PermissionHelper 
    {
        private const ulong _bannedMarker = 0;
        private const ulong _moderatorMarker = 1uL << 32;
        private const ulong _adminMarker = 1uL << 47;

        public static bool IsAdmin(this UserData user)
        {
            return ((ulong)user.PermissionLevel) >= _adminMarker;
        }

        public static string DeterminePermissionGroup(this UserData user)
        {
            ulong permissions = (ulong)user.PermissionLevel;
            if (permissions >= _adminMarker)
                return "Admin";
            if (permissions >= _moderatorMarker)
                return "Moderator";
            if (permissions == _bannedMarker)
                return "Banned";

            return null;
        }
    }
}