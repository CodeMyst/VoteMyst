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

namespace VoteMyst.PermissionSystem
{
    public class RequireGlobalPermissionAttribute : TypeFilterAttribute
    {
        public RequireGlobalPermissionAttribute(GlobalPermissions permissions) : base(typeof(RequirePermissionsAttributeImpl))
        {
            Arguments = new[] { new PermissionAuthorizationRequirement<GlobalPermissions>(permissions) };
        }

        private class RequirePermissionsAttributeImpl : Attribute, IActionFilter
        {
            private readonly PermissionAuthorizationRequirement<GlobalPermissions> permissions;
            private readonly DatabaseHelperProvider dbhelper;
            private readonly UserProfileBuilder profileBuilder;

            public RequirePermissionsAttributeImpl(PermissionAuthorizationRequirement<GlobalPermissions> permissions,
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
                UserAccount user = context.HttpContext.User.Identity.IsAuthenticated
                    ? profileBuilder.FromPrincipal(context.HttpContext.User)
                    : user = dbhelper.Users.Guest();
                
                if (!user.Permissions.HasFlag(permissions.Permissions))
                {
                    context.Result = new StatusCodeResult(StatusCodes.Status403Forbidden);
                }
            }
        }
    }
}