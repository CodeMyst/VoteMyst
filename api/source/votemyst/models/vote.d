module votemyst.models.vote;

import std.datetime;
import vibe.d;

/**
 * Represents a single vote casted by a user on an entry.
 */
public struct Vote
{
    ///
    @name("_id")
    public BsonObjectID id;

    /**
     * Id of the user that cast the vote.
     */
    public BsonObjectID authorId;

    /**
     * When the vote was cast.
     */
    public SysTime votedAt;
}
