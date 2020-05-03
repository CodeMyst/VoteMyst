using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using VoteMyst.Database;
using VoteMyst.Database.Models;

namespace VoteMyst.PermissionSystem
{
    public class RequirePermissionsAttribute : TypeFilterAttribute
    {
        public RequirePermissionsAttribute(Permissions permissions) : base(typeof(RequirePermissionsAttributeImpl))
        {
            Arguments = new[] { new PermissionAuthorizationRequirement(permissions) };
        }

        private class RequirePermissionsAttributeImpl : Attribute, IAsyncResourceFilter
        {
            private readonly PermissionAuthorizationRequirement permissions;
            private readonly DatabaseHelperProvider dbhelper;
            private readonly UserProfileBuilder profileBuilder;

            public RequirePermissionsAttributeImpl(PermissionAuthorizationRequirement permissions,
                                                   DatabaseHelperProvider dbhelper,
                                                   UserProfileBuilder profileBuilder)
            {
                this.permissions = permissions;
                this.dbhelper = dbhelper;
                this.profileBuilder = profileBuilder;
            }

            public async Task OnResourceExecutionAsync(ResourceExecutingContext context, ResourceExecutionDelegate next)
            {
                UserData user;
                if (!context.HttpContext.User.Identity.IsAuthenticated)
                {
                    user = dbhelper.Users.Guest();
                }
                else
                {
                    user = profileBuilder.FromPrincipal(context.HttpContext.User);
                }

                if (user.PermissionLevel.HasFlag(permissions.Permissions))
                {
                    await next();
                }
                else
                {
                    return;
                }
            }
        }
    }
}