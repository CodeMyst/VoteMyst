using System;

namespace VoteMyst.Database
{
    public enum AccountState
    {
        Banned = 0,

        Deleted = 10,

        Guest = 50,

        Active = 100,

        Moderator = 1000,
        
        Admin = 10000
        
    }
}