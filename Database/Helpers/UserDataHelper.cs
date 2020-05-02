using System;
using System.Linq;
using VoteMyst.Database.Models;

namespace VoteMyst.Database
{
    public class UserDataHelper
    {
        private const string DEFAULT_AVATAR = "defaultAvatar";
        private readonly VoteMystContext context;

        public UserDataHelper(VoteMystContext context) 
        {
            this.context = context;
        }

        public UserData NewUser()
        {
            var guid = Guid.NewGuid().ToString().Replace("-", "");
            UserData user = new UserData()
            {
                DisplayId = guid,
                JoinDate = DateTime.UtcNow,
                PermissionLevel = Permissions.Default,
                Username = guid,
                Avatar = DEFAULT_AVATAR
            };

            context.UserData.Add(user);

            context.SaveChanges();

            return user;
        }

        public UserData GetUser(string displayId)
            => context.UserData
                .FirstOrDefault(x => x.DisplayId.Equals(displayId));

        public UserData GetUser(int userId)
            => context.UserData
                .FirstOrDefault(x => x.UserId == userId);

        public UserData GetOrCreateUser(string displayId)
            => GetUser(displayId) ?? NewUser();

        public UserData GetOrCreateUser(int userId)
            => GetUser(userId) ?? NewUser();

        public bool DeleteUser(int userId)
            => DeleteUser(GetUser(userId));

        public bool DeleteUser(UserData user)
        {
            if (user != null) 
                context.UserData.Remove(user);

            return context.SaveChanges() > 0;
        }

        public bool AddPermission(UserData user, Permissions permissions)
        {
            user.PermissionLevel |= permissions;

            context.UserData.Update(user);

            return context.SaveChanges() > 0;
        }

        public bool RemovePermission(UserData user, Permissions permissions)
        {
            user.PermissionLevel ^= permissions;

            context.UserData.Update(user);

            return context.SaveChanges() > 0;
        }

        public bool SetPermission(UserData user, Permissions permissions)
        {
            user.PermissionLevel = permissions;

            context.UserData.Update(user);

            return context.SaveChanges() > 0;
        }
    }

}