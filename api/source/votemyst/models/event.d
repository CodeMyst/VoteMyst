module votemyst.models.event;

import std.datetime;
import vibe.d;
import votemyst.models;

/**
 * Represents an event that contains entries posted by users.
 */
public struct Event
{
    ///
    @name("_id")
    public BsonObjectID id;

    /**
     * Publically visible ID of the event.
     */
    public string displayId;

    /**
     * Vanity URL. If empty then the `displayId` is used.
     */
    @optional
    public string vanityUrl;

    ///
    public string title;

    ///
    public string description;

    ///
    public EventType type;

    ///
    public EventSettings settings = EventSettings.defaultSettings;

    /**
     * Time when the event will be publically revealed to non-hosts.
     */
    public SysTime revealDate;

    /**
     * When the submissions of the event are open.
     */
    public SysTime submissionStartDate;

    /**
     * When the submissions of the event are closed.
     */
    public SysTime submissionEndDate;

    /**
     * When the voting for the event is over.
     */
    public SysTime voteEndDate;

    /**
     * List of event hosts.
     */
    public BsonObjectID[] hostIds;
}
