using System;
using System.ComponentModel.DataAnnotations;

namespace VoteMyst.Database
{
    [Flags]
    public enum EventSettings : ulong
    {
        [Display(Name = null)]
        None = 0,

        [Display(Name = "Randomize Entries", Description = "Randomizes the order of the entries in the event before showing them, to avoid biasing by upload time.")]
        RandomizeEntries = 1uL << 0,
        [Display(Name = "Exclude Staff From Winning", Description = "Excludes staff members of the event from appearing on the leaderboard.")]
        ExcludeStaffFromWinning = 1uL << 1,
        [Display(Name = "Require Vote To Win", Description = "Requires that participants cast a single vote to be eligible to win.")]
        RequireVoteToWin = 1uL << 2,

        [Display(Name = null)]
        Default = RandomizeEntries | ExcludeStaffFromWinning,
    }
}