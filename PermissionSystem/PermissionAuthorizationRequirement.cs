using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using VoteMyst.Database;

namespace VoteMyst.PermissionSystem
{
    public class PermissionAuthorizationRequirement : IAuthorizationRequirement
    {
        public Permissions Permissions;

        public PermissionAuthorizationRequirement(Permissions permissions)
        {
            Permissions = permissions;
        }
    }
}