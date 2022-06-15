module votemyst.models.entry;

import std.datetime;
import vibe.d;

/**
 * Represents one submission to an event by a user. Different entry types (based on the event type) should use these base fields.
 */
public struct BaseEntry
{
    ///
    @name("_id")
    public BsonObjectID id;

    /**
     * The event ID to which this entry belongs to.
     */
    public BsonObjectID eventId;

    /**
     * ID of the user which submitted this entry.
     */
    public BsonObjectID authorId;

    /**
     * When the entry was submitted.
     */
    public SysTime submitDate;
}
