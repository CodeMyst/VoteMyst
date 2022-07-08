module votemyst.models.entry;

import std.datetime;
import vibe.data.bson;
import vibe.data.json;
import vibe.data.serialization;
import votemyst.models;

/**
 * Represents one submission to an event by a user. Different entry types (based on the event type) should use these base fields.
 *
 * This is a class so we can have inheritance (multiple types of entries).
 */
public class Entry
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

    /**
     * List of all votes on this entry.
     */
    public Vote[] votes;
}
