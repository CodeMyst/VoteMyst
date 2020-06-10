namespace VoteMyst.Database
{
    /// <summary>
    /// Indicates that a database entity can be displayed via a public ID.
    /// </summary>
    public interface IPublicDisplayable : IDatabaseEntity
    {
        /// <summary>
        /// The ID via which the entity should be displayed.
        /// </summary>
        string DisplayID { get; set; }
    }
}