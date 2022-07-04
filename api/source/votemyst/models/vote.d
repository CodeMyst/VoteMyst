module votemyst.models.vote;

import std.datetime;
import vibe.d;
import votemyst.models;

/**
 * Represents a single vote cast by a user on an entry.
 *
 * This is a class so we can have inheritance (multiple types of votes).
 */
public class Vote
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

/**
 * Represents a single vote cast by a user on an entry.
 *
 * Every entry can be upvoted (no downvote).
 *
 * Stores the same stuff as `Vote`, no need for extra data.
 */
public class UpvoteVote { }

/**
 * Represents a single vote cast by a user on an entry.
 *
 * Every entry can be rated from 1-5.
 */
public class SimpleVote
{
    /**
     * 1-5 rating.
     */
    public int rating;
}

/**
 * Represents a single vote cast by a user on an entry.
 *
 * Every entry can be rated in multiple categories, 1-5.
 */
public class CategoriesVote
{
    /**
     * All categories and their ratings.
     */
    public int[string] ratedCategories;
}
