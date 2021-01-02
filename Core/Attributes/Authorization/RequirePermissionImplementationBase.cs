using System;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Http;

using VoteMyst.Database;

namespace VoteMyst.Authorization
{
    public abstract class RequirePermissionImplementationBase<T> : IActionFilter where T : Enum
    {
        protected UserAccount User { get; private set; }
        protected HttpContext Context { get; private set; }

        protected readonly UserProfileBuilder _profileBuilder;
        protected readonly DatabaseHelperProvider _databaseHelpers;
        protected readonly T _permissions;

        public RequirePermissionImplementationBase(UserProfileBuilder profileBuilder, DatabaseHelperProvider databaseHelpers, T permissions)
        {
            _profileBuilder = profileBuilder;
            _databaseHelpers = databaseHelpers;
            _permissions = permissions;
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            Context = context.HttpContext;
            User = Context.User.Identity.IsAuthenticated
                ? _profileBuilder.FromPrincipal(Context.User)
                : _databaseHelpers.Users.Guest();

            if (!CheckAuthorized())
            {
                context.Result = new StatusCodeResult(StatusCodes.Status403Forbidden);
            }

            Context = null;
            User = null;
        }

        protected abstract bool CheckAuthorized();
    }
}