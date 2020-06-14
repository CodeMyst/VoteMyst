using System;

using Microsoft.AspNetCore.Authorization;

using VoteMyst.Database;

namespace VoteMyst.PermissionSystem
{
    public class PermissionAuthorizationRequirement<T> : IAuthorizationRequirement where T : Enum
    {
        public T Permissions { get; set; }

        public PermissionAuthorizationRequirement(T permissions)
        {
            Permissions = permissions;
        }
    }
}