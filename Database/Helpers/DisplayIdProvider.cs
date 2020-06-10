using System;
using System.Reflection;

namespace VoteMyst.Database
{
    /// <summary>
    /// Provides unique DisplayIDs for database entities.
    /// </summary>
    public static class DisplayIDProvider
    {
        private const int _defaultLength = 28;

        /// <summary>
        /// Injects a DisplayID into the specified entity using the default length of 28 or a specified length if overriden.
        /// </summary>
        public static void InjectDisplayId(IPublicDisplayable displayable)
        {
            int length = displayable.GetType()
                .GetProperty(nameof(displayable.DisplayID))
                .GetCustomAttribute<DisplayIDAttribute>()?.Length ?? _defaultLength;

            displayable.DisplayID = GenerateDisplayID(length);
        }

        private static string GenerateDisplayID(int length)
            => Guid.NewGuid().ToString().Replace("-", "").Substring(0, length);
    }
}