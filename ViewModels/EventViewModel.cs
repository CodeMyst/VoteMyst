using System.Linq;

using VoteMyst.Database;

namespace VoteMyst.ViewModels
{
    public class EventViewModel
    {
        public Event Event { get; }

        public EventPermissions EventPermissions { get; }
        public EventState EventState { get; }

        public bool IsUserSiteAdmin { get; }

        public string[] Hosts { get; }

        public EventViewModel (Event ev, UserAccount currentUser, DatabaseHelperProvider databaseHelpers)
        {
            Event = ev;

            EventPermissions = databaseHelpers.Events.GetUserPermissionsForEvent(currentUser, ev);
            EventState = ev.GetCurrentState();

            IsUserSiteAdmin = currentUser.Permissions.HasFlag(GlobalPermissions.ManageAllEvents);

            Hosts = databaseHelpers.Events.GetEventHosts(ev)
                .Select(h => $"<a href=\"{h.GetUrl()}\">{h.Username}</a>")
                .ToArray();
        }
    }
}