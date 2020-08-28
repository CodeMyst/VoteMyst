using VoteMyst.Database;
using VoteMyst.Controllers;

namespace VoteMyst.ViewModels
{
    public class EntryPlaceViewModel : EntryViewModel
    {
        public EventsController.Leaderboard.Place Place { get; }

        public EntryPlaceViewModel(EventsController.Leaderboard.Place place, UserAccount currentUser, DatabaseHelperProvider databaseHelpers)
            : base(place.Entry, currentUser, databaseHelpers)
        {
            Place = place;
        }
    }
}