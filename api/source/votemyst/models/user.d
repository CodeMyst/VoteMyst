module votemyst.models.user;

import std.datetime;
import vibe.d;
import votemyst.models;

///
public struct User
{
    ///
    @name("_id")
    public BsonObjectID id;

    ///
    public string username;

    ///
    public SysTime joinDate;

    ///
    public UserRole role;

    /**
     * List of user IDs for different OAuth providers. IDs are hashed.
     */
    public string[string] oauthProviderIds;

    /**
     * Link to the user's avatar. Can be either third party (hosted by an OAuth provider)
     * or hosted on this site (when changing the default avatar).
     */
    public string avatarUrl;
}
