using System;
using System.IO;
using System.Linq;
using VoteMyst.Database.Models;

namespace VoteMyst.Database
{
    public class UserDataHelper
    {
        private const string GUEST_USER_NAME = "Guest User";
        private const string DELETED_USER_NAME = "Deleted User";
        private readonly VoteMystContext _context;
        private readonly AuthorizationHelper _authHelper;
        private readonly AvatarHelper _avatarHelper;

        public UserDataHelper(VoteMystContext context, AuthorizationHelper authHelper, AvatarHelper avatarHelper) 
        {
            this._context = context;
            this._authHelper = authHelper;
            this._avatarHelper = avatarHelper;
        }

        public UserData NewUser()
        {
            var guid = Guid.NewGuid().ToString().Replace("-", "");
            UserData user = new UserData()
            {
                DisplayId = guid,
                Username = guid,
                PermissionLevel = AccountState.Active.GetDefaultPermissions(),
                JoinDate = DateTime.UtcNow,
                AccountState = AccountState.Active
            };

            _context.UserData.Add(user);

            _context.SaveChanges();

            return user;
        }

        public UserData Guest()
            => new UserData 
            {
                UserId = -1,
                DisplayId = null,
                Username = GUEST_USER_NAME,
                PermissionLevel = AccountState.Guest.GetDefaultPermissions(),
                JoinDate = DateTime.Today,
                AccountState = AccountState.Guest
            };

        public UserData GetUser(string displayId)
            => _context.UserData
                .FirstOrDefault(x => x.DisplayId.Equals(displayId));

        public UserData GetUser(int userId)
            => _context.UserData
                .FirstOrDefault(x => x.UserId == userId);

        public UserData GetOrCreateUser(string displayId)
            => GetUser(displayId) ?? NewUser();

        public UserData GetOrCreateUser(int userId)
            => GetUser(userId) ?? NewUser();

        public bool WipeUser(int userId) 
        {
            UserData user = GetUser(userId);

            user.Username = DELETED_USER_NAME;

            string avatarUrl = _avatarHelper.GetAbsoluteAvatarUrl(user);
            
            if (avatarUrl != null && File.Exists(avatarUrl))
                File.Delete(avatarUrl); 

            Authorization[] auths = _authHelper.GetAllAuthorizationsOfUser(user.UserId);
            
            foreach (Authorization auth in auths)
                auth.Valid = false;

            return _context.SaveChanges() > 0;
        }

        public bool AddPermission(UserData user, Permissions permissions)
        {
            user.PermissionLevel |= permissions;

            _context.UserData.Update(user);

            return _context.SaveChanges() > 0;
        }

        public bool RemovePermission(UserData user, Permissions permissions)
        {
            user.PermissionLevel ^= permissions;

            _context.UserData.Update(user);

            return _context.SaveChanges() > 0;
        }

        public bool SetPermission(UserData user, Permissions permissions)
        {
            user.PermissionLevel = permissions;

            _context.UserData.Update(user);

            return _context.SaveChanges() > 0;
        }

        public bool SetAccountState(UserData user, AccountState accountState)
        {
            user.AccountState = accountState;

            return _context.SaveChanges() > 0;
        }
    }

}