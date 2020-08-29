namespace VoteMyst.Maintenance
{
    /// <summary>
    /// Provides constants for the <see cref="MaintenanceModeMiddleware"/>.
    /// </summary>
    public static class MaintenanceModeConstants
    {
        /// <summary>
        /// The configuration key that indicates if the site is in maintenance mode.
        /// </summary>
        public const string ConfigurationKey = "maintenance";
        /// <summary>
        /// The path that the maintenance page is at.
        /// </summary>
        public const string PagePath = "/maintenance";
    }
}
