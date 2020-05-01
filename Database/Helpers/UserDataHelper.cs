using System;
using System.Linq;
using VoteMyst.Database.Models;

namespace VoteMyst.Database
{
    public static class UserDataHelper
    {
        private static readonly VoteMystContext context = VoteMystContext.Context;

        public static bool NewUser(ulong snowflake, out UserData user)
        {
            user = new UserData()
            {
                Snowflake = snowflake,
                JoinDate = DateTime.UtcNow,
                PermissionLevel = Permissions.Default
            };

            context.UserData.Add(user);

            return context.SaveChanges() > 0;
        }

        public static UserData GetOrCreateUser(ulong snowflake)
        {
            var userData = context.UserData.FirstOrDefault(x => x.Snowflake == snowflake);

            if (userData == null && !NewUser(snowflake, out userData))
                userData = null;

            return userData;
        }

        public static bool DeleteUser(ulong snowflake)
            => DeleteUser(GetOrCreateUser(snowflake));

        public static bool DeleteUser(UserData user)
        {
            context.UserData.Remove(user);

            return context.SaveChanges() > 0;
        }

        public static bool AddPermission(ulong snowflake, Permissions permissions)
        {
            UserData user = GetOrCreateUser(snowflake);

            user.PermissionLevel |= permissions;

            context.UserData.Update(user);

            return context.SaveChanges() > 0;
        }

        public static bool RemovePermission(ulong snowflake, Permissions permissions)
        {
            UserData user = GetOrCreateUser(snowflake);

            user.PermissionLevel ^= permissions;

            context.UserData.Update(user);

            return context.SaveChanges() > 0;
        }

        public static bool SetPermission(ulong snowflake, Permissions permissions)
        {
            UserData user = GetOrCreateUser(snowflake);

            user.PermissionLevel = permissions;

            context.UserData.Update(user);

            return context.SaveChanges() > 0;
        }
    }

}