using System;

namespace VoteMyst.Database
{
    [Flags]
    public enum Permissions : ulong
    {
        // Users
        SubmitEntries = 1uL << 0,
        SubmitVotes = 1uL << 1,
        ViewEntries = 1uL << 2,
        ViewEvents = 1uL << 3,
        EditEntries = 1uL << 4,
        ViewUsers = 1uL << 5,

        AllowWinning = 1uL << 63,

        // Moderation
        DeleteEntries = 1uL << 32,
        DeleteVotes = 1uL << 33,

        // Administration
        ModifyUsers = 1uL << 47,
        CreateEvents = 1uL << 48,
        EditEvents = 1uL << 49,
        DeleteEvents = 1uL << 50,

        // Groups
        Banned = 0,
        Guest = ViewEntries | ViewEvents,
        Default = Guest | SubmitEntries | SubmitVotes | EditEntries | ViewUsers | AllowWinning,
        Moderator = Default | DeleteEntries | DeleteVotes ^ AllowWinning,
        Admin = ulong.MaxValue ^ AllowWinning
    }
}