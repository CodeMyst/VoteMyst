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

        public UserData GetOrCreateUser(string displayId)
            => context.UserData
                .FirstOrDefault(x => x.DisplayId.Equals(displayId))
                ?? NewUser();

        public UserData GetOrCreateUser(int userId)
            => context.UserData
                .FirstOrDefault(x => x.UserId == userId) 
                ?? NewUser();
        

        public bool DeleteUser(int userId)
            => DeleteUser(GetOrCreateUser(userId));

        public bool DeleteUser(UserData user)
        {
            context.UserData.Remove(user);

            return context.SaveChanges() > 0;
        }

        public bool AddPermission(int userId, Permissions permissions)
        {
            UserData user = GetOrCreateUser(userId);

            user.PermissionLevel |= permissions;

            context.UserData.Update(user);

            return context.SaveChanges() > 0;
        }

        public bool RemovePermission(int userId, Permissions permissions)
        {
            UserData user = GetOrCreateUser(userId);

            user.PermissionLevel ^= permissions;

            context.UserData.Update(user);

            return context.SaveChanges() > 0;
        }

        public bool SetPermission(int userId, Permissions permissions)
        {
            UserData user = GetOrCreateUser(userId);

            user.PermissionLevel = permissions;

            context.UserData.Update(user);

            return context.SaveChanges() > 0;
        }
    }

}