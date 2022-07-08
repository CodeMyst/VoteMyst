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
    public EventSettings settings = EventSettings.defaultSettings;

    /**
     * How the entries are voted on.
     */
    public VoteType voteType = VoteType.upvote;

    /**
     * List of all categories to be voted on.
     *
     * This field is only applicable when the `VoteType` is `categories`.
     */
    @optional
    public string[] categories;

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
