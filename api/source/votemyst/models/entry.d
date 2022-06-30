module votemyst.models.entry;

import std.datetime;
import vibe.data.bson;
import vibe.data.json;
import vibe.data.serialization;

/**
 * Represents one submission to an event by a user. Different entry types (based on the event type) should use these base fields.
 *
 * This is a template instead of a struct (and "inheiriting" with alias this) because MongoDB won't serialize it right.
 */
public template BaseEntryTmpl()
{
    import std.datetime;
    import vibe.data.bson;
    import vibe.data.json;
    import vibe.data.serialization;

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

/**
 * Represents one submission to an event by a user. Different entry types (based on the event type) should use these base fields.
 *
 * This is a template instead of a struct (and "inheiriting" with alias this) because MongoDB won't serialize it right.
 */
public struct BaseEntry
{
    mixin BaseEntryTmpl;
}
