using System;

namespace VoteMyst
{
    public static class TimeHelpers
    {
        private static readonly DateTime _unixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        public static ulong ToUnixSeconds(this DateTime dateTime)
        {
            return (ulong)Math.Floor((dateTime - _unixEpoch).TotalSeconds);
        }
    }
}