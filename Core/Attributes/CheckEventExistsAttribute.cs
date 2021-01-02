using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

using VoteMyst.Database;

namespace VoteMyst
{
    public class CheckEventExistsAttribute : TypeFilterAttribute
    {
        public CheckEventExistsAttribute(string eventIdKey = "id") : base(typeof(CheckDatabaseEntityImplementation))
        {
            Arguments = new object[] { eventIdKey };
        }

        private class CheckDatabaseEntityImplementation : IActionFilter
        {
            private readonly DatabaseHelperProvider _databaseHelpers;
            private readonly string _eventIdKey;

            public CheckDatabaseEntityImplementation(DatabaseHelperProvider databaseHelpers, string eventIdKey)
            {
                _databaseHelpers = databaseHelpers;
                _eventIdKey = eventIdKey;
            }

            public void OnActionExecuted(ActionExecutedContext context)
            {
            }
            public void OnActionExecuting(ActionExecutingContext context)
            {
                string eventId = context.HttpContext.Request.RouteValues[_eventIdKey].ToString();
                if (_databaseHelpers.Events.GetEventByUrl(eventId) == null)
                {
                    context.Result = new StatusCodeResult(StatusCodes.Status404NotFound);
                }
            }
        }
    }
}
