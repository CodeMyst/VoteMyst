namespace VoteMyst.Database
{
    /// <summary>
    /// Represents an entity inside a database.
    /// </summary>
    public interface IDatabaseEntity
    {
        /// <summary>
        /// The internal ID which is used by the database and for fast querying.
        /// </summary>
        int ID { get; }
    }
}