using Microsoft.AspNetCore.Mvc;

using VoteMyst.Database;

namespace VoteMyst.Authorization
{
    public class RequireEventPermissionAttribute : TypeFilterAttribute
    {
        public RequireEventPermissionAttribute(EventPermissions permissions, string eventIdKey = "id") : base(typeof(RequireEventPermissionImplementation))
        {
            Arguments = new object[] { permissions, eventIdKey };
        }

        private class RequireEventPermissionImplementation : RequirePermissionImplementationBase<EventPermissions>
        {
            private readonly string _eventIdKey;

            public RequireEventPermissionImplementation(UserProfileBuilder profileBuilder, DatabaseHelperProvider databaseHelpers, EventPermissions permissions, string eventIdKey)
                : base(profileBuilder, databaseHelpers, permissions)
            {
                _eventIdKey = eventIdKey;
            }

            protected override bool CheckAuthorized()
            {
                string eventId = Context.Request.RouteValues[_eventIdKey].ToString();
                Event e = _databaseHelpers.Events.GetEventByUrl(eventId);

                return e != null && _databaseHelpers.Events.GetUserPermissionsForEvent(User, e).HasFlag(_permissions);
            }
        }
    }
}