using System.ComponentModel.DataAnnotations;

namespace VoteMyst.Database
{
    public enum AccountBadge
    {
        None,
        Banned = 1,
        [Display(Name = "Site Moderator")]
        SiteModerator = 10,
        [Display(Name = "Site Admin")]
        SiteAdministrator = 100
    }
}