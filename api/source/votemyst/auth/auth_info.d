module votemyst.auth.auth_info;

import vibe.d;
import votemyst.models;

@safe:

/**
 * Info about the currently logged in user.
 */
public struct AuthInfo
{
    ///
    public BsonObjectID id;

    ///
    public string username;

    ///
    public UserRole role;

    ///
    public bool loggedIn;

    ///
    public bool isAdmin()
    {
        return loggedIn && role == UserRole.admin;
    }

    ///
    public bool isLoggedIn()
    {
        return loggedIn;
    }
}
