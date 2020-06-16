using System;
using System.ComponentModel.DataAnnotations;

namespace VoteMyst.Database
{
    [Flags]
    public enum GlobalPermissions : ulong
    {
        [Display(Name = null)]
        None = 0,

        [Display(Name = "Participate In Events", Description = "Members with this permission are allowed to participate in events hosted on the site.")]
        ParticipateInEvents = 1uL << 0,
        [Display(Name = "Manage Self", Description = "Members with this permission are allowed to edit their profile by changing their usernames or avatars.")]
        ManageSelf = 1uL << 1,
        [Display(Name = "Create Events", Description = "Members with this permission are allowed to create and host new events.")]
        CreateEvents = 1uL << 4,

        [Display(Name = "Manage All Events", Description = "Members with this permission are allowed to manage events that they are not declared host of.")]
        ManageAllEvents = 1uL << 8,
        [Display(Name = "Manage Users", Description = "Members with this permission are allowed to manage and edit other users.")]
        ManageUsers = 1uL << 9,

        [Display(Name = null)]
        Default = ParticipateInEvents | ManageSelf,
        [Display(Name = null)]
        SiteAdministrator = Default | CreateEvents | ManageAllEvents | ManageUsers
    }
}