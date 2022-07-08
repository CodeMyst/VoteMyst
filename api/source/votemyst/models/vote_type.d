module votemyst.models.vote_type;

/**
 * How the entries in the event are voted on.
 */
public enum VoteType
{
    upvote, // every entry can be upvoted (no downvoting)
    simple, // every entry can be rated from 1-5
    categories // multiple categories, each category can be rated from 1-5
}
