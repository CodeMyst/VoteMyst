using System;
using System.Linq;
using System.Collections.Generic;

namespace VoteMyst.Database
{
    /// <summary>
    /// Provides utility to handle <see cref="Event"/>s.
    /// </summary>
    public class EventHelper
    {
        private readonly VoteMystContext context;

        public EventHelper(VoteMystContext context)
        {
            this.context = context;
        }

        /// <summary>
        /// Creates an event with the given user specified as host.
        /// </summary>
        public Event CreateEvent(Event e, UserAccount host)
        {
            DisplayIDProvider.InjectDisplayId(e);

            context.Events.Add(e);
            context.SaveChanges();

            RegisterUserAsHost(host, e);

            return e;
        }

        /// <summary>
        /// Retrieves an event by its URL. Note that both the <see cref="Event.DisplayID"/> and the <see cref="Event.URL"/> apply here.
        /// </summary>
        public Event GetEventByUrl(string url)
            => context.Events
                .AsEnumerable()
                .FirstOrDefault(x => x.GetValidDisplayUrl().Equals(url, StringComparison.InvariantCultureIgnoreCase));

        /// <summary>
        /// Returns all events that start between the two given dates.
        /// </summary>
        public Event[] GetEventsBetween(DateTime start, DateTime end)
            => context.Events
                .Where(x => x.StartDate >= start)
                .Where(x => x.StartDate <= end)
                .ToArray();

        /// <summary>
        /// Retrieves all events that the user participated in.
        /// </summary>
        public Event[] GetEventsUserParticipatedIn(UserAccount user)
            => user.Entries.Select(e => e.Event).ToArray();

        /// <summary>
        /// Retrieves all events, grouped by their respective state.
        /// </summary>
        public Dictionary<EventState, Event[]> GetAllEventsGrouped()
        {
            EventState[] states = (EventState[])Enum.GetValues(typeof(EventState));
            IEnumerable<KeyValuePair<EventState, Event>> eventsWithStates = context.Events
                .OrderByDescending(e => e.StartDate)
                .Select(e => new KeyValuePair<EventState, Event>(e.GetCurrentState(), e));
            return states.ToDictionary(state => state, state => eventsWithStates.Where(e => e.Key == state).Select(e => e.Value).ToArray());
        }

        /// <summary>
        /// Returns all events that start after the given date.
        /// </summary>
        public Event[] GetAllEventsAfter(DateTime date)
            => context.Events
                .Where(x => x.StartDate >= date)
                .ToArray();

        /// <summary>
        /// Returns all events that start before the given date.
        /// </summary>
        public Event[] GetAllEventsBefore(DateTime date)
            => context.Events
                .Where(x => x.StartDate <= date)
                .ToArray();

        /// <summary>
        /// Returns all events that are finished before the given date.
        /// </summary>
        public Event[] GetAllEventsFinishedBefore(DateTime date)
            => context.Events
                .Where(x => x.VoteEndDate < date)
                .OrderByDescending(x => x.StartDate)
                .ToArray();

        /// <summary>
        /// Get all events that are either ongoing or on the voting phase.
        /// </summary>
        public Event[] GetCurrentEvents()
            => context.Events
                .Where(x => x.StartDate <= DateTime.UtcNow)
                .Where(x => x.VoteEndDate > DateTime.UtcNow)
                .OrderBy(x => x.VoteEndDate)
                .ToArray();

        /// <summary>
        /// Retrieves all events that are planned and visible by everyone.
        /// </summary>
        /// <returns></returns>
        public Event[] GetPlannedEvents()
            => context.Events
                .Where(x => x.StartDate > DateTime.UtcNow && x.RevealDate <= DateTime.UtcNow)
                .ToArray();

        /// <summary>
        /// Retrieves all events that are planned and visible by the given user.
        /// </summary>
        public Event[] GetPlannedEvents(UserAccount user)
            => context.Events
                .Where(x => x.StartDate > DateTime.UtcNow)
                .OrderBy(x => x.StartDate)
                .AsEnumerable()
                .Where(x => user.Permissions.HasFlag(GlobalPermissions.ManageAllEvents)
                    || GetUserPermissionsForEvent(user, x).HasFlag(EventPermissions.EditEventSettings)
                    || x.RevealDate <= DateTime.UtcNow)
                .ToArray();

        /// <summary>
        /// Checks if the specified user can win the given event, based on his <see cref="EventPermissions"/>.
        /// </summary>
        public bool CanUserWin(UserAccount user, Event e)
        {
            EventPermissions antiWinningPermissions = EventPermissions.EditEventSettings | EventPermissions.ManageEntries | EventPermissions.ManageVotes;
            return ((GetUserPermissionsForEvent(user, e) & antiWinningPermissions) == 0);
        }

        /// <summary>
        /// Retrieves the users <see cref="EventPermissions"/> for the given event.
        /// </summary>
        public EventPermissions GetUserPermissionsForEvent(UserAccount user, Event e)
            => context.EventPermissionModifiers
                .FirstOrDefault(x => x.Event.ID == e.ID && x.User.ID == user.ID)?.Permissions ?? EventPermissions.Default;

        /// <summary>
        /// Returns the host users of the given event.
        /// </summary>
        public UserAccount[] GetEventHosts(Event e)
            => context.EventPermissionModifiers
                .Where(x => x.Event.ID == e.ID && x.Permissions == EventPermissions.Host)
                .Join(context.UserAccounts,
                    evm => evm.User.ID,
                    account => account.ID,
                    (evm, account) => account)
                .ToArray();

        /// <summary>
        /// Checks if the user can view the specified (hidden) event.
        /// </summary>
        public bool CanViewHiddenEvent(UserAccount user, Event e) 
            => user.Permissions.HasFlag(GlobalPermissions.ManageAllEvents) 
                || GetEventHosts(e).Contains(user);

        /// <summary>
        /// Returns all events in which the user is registered as host.
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public Event[] GetHostedEvents(UserAccount user)
            => context.EventPermissionModifiers
                .Where(x => x.User.ID == user.ID && x.Permissions == EventPermissions.Host)
                .Select(x => x.Event)
                .ToArray();

        /// <summary>
        /// Registers the given user as an event host.
        /// </summary>
        public bool RegisterUserAsHost(UserAccount user, Event e) 
        {
            EventPermissionModifier modifier = context.EventPermissionModifiers.FirstOrDefault(m => m.Event.ID == e.ID && m.User.ID == user.ID);

            if (modifier == null)
            {
                context.EventPermissionModifiers.Add(new EventPermissionModifier
                {
                    Event = e,
                    User = user,
                    Permissions = EventPermissions.Host
                });
            }
            else
            {
                modifier.Permissions = EventPermissions.Host;
            }

            return context.SaveChanges() > 0;
        }
        public bool RemoveUserAsHost(UserAccount user, Event e)
        {
            EventPermissionModifier modifier = context.EventPermissionModifiers
                .FirstOrDefault(m => m.Event.ID == e.ID && m.User.ID == user.ID);

            if (modifier != null)
            {
                context.EventPermissionModifiers.Remove(modifier);
                context.SaveChanges();
                return true;
            }
            return false;
        }
    }
}