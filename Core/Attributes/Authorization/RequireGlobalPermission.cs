using Microsoft.AspNetCore.Mvc;

using VoteMyst.Database;

namespace VoteMyst.Authorization
{
    public class RequireGlobalPermissionAttribute : TypeFilterAttribute
    {
        public RequireGlobalPermissionAttribute(GlobalPermissions permissions) : base(typeof(RequireGlobalPermissionImplementation))
        {
            Arguments = new object[] { permissions };
        }

        private class RequireGlobalPermissionImplementation : RequirePermissionImplementationBase<GlobalPermissions>
        {
            public RequireGlobalPermissionImplementation(UserProfileBuilder profileBuilder, DatabaseHelperProvider databaseHelpers, GlobalPermissions permissions)
                : base(profileBuilder, databaseHelpers, permissions) { }

            protected override bool CheckAuthorized()
            {
                return User.Permissions.HasFlag(_permissions);
            }
        }
    }
}