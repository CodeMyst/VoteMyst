using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using VoteMyst.Controllers;
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

        private class RequirePermissionsAttributeImpl : Attribute, IActionFilter
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

            public void OnActionExecuted(ActionExecutedContext context)
            {
            }

            public void OnActionExecuting(ActionExecutingContext context)
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

                if (user.IsBanned() || !user.PermissionLevel.HasFlag(permissions.Permissions))
                {
                    context.Result = new StatusCodeResult(StatusCodes.Status403Forbidden);
                }
            }
        }
    }
}