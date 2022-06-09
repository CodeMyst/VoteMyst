module votemyst.models.event_settings;

///
public enum EventSettings : ulong
{
    none = 0,

    randomizeEntries = 1uL << 0,
    excludeStaffFromWinning = 1uL << 1,
    requireVoteToWin = 1uL << 2,

    defaultSettings = randomizeEntries | excludeStaffFromWinning
}
