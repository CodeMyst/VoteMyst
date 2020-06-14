using System;

namespace VoteMyst.Database
{
    [Flags]
    public enum EventPermissions : ulong
    {
        None = 0,
        SpectateEvent = 1uL << 0,
        ParticipateInEvent = 1uL << 1,
        ManageEntries = 1uL << 2,
        ManageVotes = 1uL << 3,
        EditEventSettings = 1uL << 4,

        Default = SpectateEvent | ParticipateInEvent,
        Host = Default | ManageEntries | ManageVotes | EditEventSettings
    }
}