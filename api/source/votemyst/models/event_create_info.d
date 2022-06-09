module votemyst.models.event_create_info;

import std.datetime;
import vibe.d;
import votemyst.models;

/**
 * Struct used for creating new events.
 */
public struct EventCreateInfo
{
    /**
     * Vanity URL. If empty then the `id` is used.
     */
    @optional
    public string vanityUrl;

    ///
    public string title;

    /**
     * Description that is showed on the events listing page.
     */
    public string shortDescription;

    ///
    public string description;

    ///
    public EventType type;

    ///
    public EventSettings settings;

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
}
