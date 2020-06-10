using System;

namespace VoteMyst.Database
{
    [Flags]
    public enum GlobalPermissions : ulong
    {
        None = 0,
        ParticipateInEvents = 1uL << 0,
        CreateEvents = 1uL << 1,
        ManageAllEvents = 1uL << 2,
        ManageUsers = 1uL << 3,

        Default = ParticipateInEvents,
        SiteAdministrator = Default | CreateEvents | ManageAllEvents | ManageUsers
    }
}