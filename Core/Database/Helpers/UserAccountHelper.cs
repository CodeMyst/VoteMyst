using System;
using System.IO;
using System.Linq;

namespace VoteMyst.Database
{
    /// <summary>
    /// Provides utility to handle <see cref="UserAccount"/>s.
    /// </summary>
    public class UserAccountHelper
    {
        private const string GUEST_USER_NAME = "Guest User";
        private const string DELETED_USER_NAME = "Deleted User";

        private readonly VoteMystContext _context;
        private readonly AvatarHelper _avatarHelper;

        public UserAccountHelper(VoteMystContext context, AvatarHelper avatarHelper) 
        {
            _context = context;
            _avatarHelper = avatarHelper;
        }

        /// <summary>
        /// Creates a new user account with the specified username.
        /// </summary>
        public UserAccount NewUser(string username)
        {
            UserAccount user = new UserAccount()
            {
                Username = username,
                Permissions = GlobalPermissions.Default,
                JoinDate = DateTime.UtcNow
            };
            
            DisplayIDProvider.InjectDisplayId(user);

            _context.UserAccounts.Add(user);
            _context.SaveChanges();

            return user;
        }

        /// <summary>
        /// Creates a temporary guest account.
        /// </summary>
        public UserAccount Guest()
            => new UserAccount 
            {
                ID = -1,
                DisplayID = null,
                Username = GUEST_USER_NAME,
                Permissions = GlobalPermissions.None,
                JoinDate = DateTime.UtcNow
            };

        /// <summary>
        /// Queries the user accounts.
        /// </summary>
        public UserAccount[] SearchUsers(string query)
            => _context.UserAccounts
                .Where(x => x.Username.Contains(query, StringComparison.InvariantCultureIgnoreCase))
                .ToArray();

        /// <summary>
        /// Wipes the specified user account.
        /// </summary>
        public bool WipeUser(UserAccount user) 
        {
            user.Username = DELETED_USER_NAME;

            string avatarUrl = _avatarHelper.GetAbsoluteAvatarUrl(user);
            if (avatarUrl != null && File.Exists(avatarUrl))
                File.Delete(avatarUrl);
                        
            foreach (Authorization auth in user.Authorizations)
                auth.Valid = false;

            return _context.SaveChanges() > 0;
        }

        /// <summary>
        /// Adds the specified permission to the user.
        /// </summary>
        public bool AddPermission(UserAccount user, GlobalPermissions permissions)
        {
            user.Permissions |= permissions;
            _context.UserAccounts.Update(user);
            return _context.SaveChanges() > 0;
        }

        /// <summary>
        /// Removes the specified permission from the user.
        /// <para>Note that the permission will be added if the user does not have it.</para>
        /// </summary>
        public bool RemovePermission(UserAccount user, GlobalPermissions permissions)
        {
            user.Permissions ^= permissions;
            _context.UserAccounts.Update(user);
            return _context.SaveChanges() > 0;
        }

        /// <summary>
        /// Sets the permissions of the specified user.
        /// </summary>
        public bool SetPermission(UserAccount user, GlobalPermissions permissions)
        {
            user.Permissions = permissions;
            _context.UserAccounts.Update(user);
            return _context.SaveChanges() > 0;
        }

        /// <summary>
        /// Sets the badge of the user.
        /// </summary>
        public bool SetBadge(UserAccount user, AccountBadge badge)
        {
            user.AccountBadge = badge;
            _context.UserAccounts.Update(user);
            return _context.SaveChanges() > 0;
        }
    }
}