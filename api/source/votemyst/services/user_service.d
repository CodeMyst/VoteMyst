module votemyst.services.user_service;

import std.typecons;
import vibe.d;
import votemyst.constants;
import votemyst.models;
import votemyst.services;
import votemyst.utils;

@safe:

/**
 * Service for handling user stuff.
 */
public class UserService
{
    private MongoService mongoService;

    ///
    public this(MongoService mongoService)
    {
        this.mongoService = mongoService;
    }

    ///
    public Nullable!User findById(BsonObjectID id)
    {
        return mongoService.findOneById!User(id);
    }

    ///
    public bool existsById(BsonObjectID id)
    {
        return !findById(id).isNull();
    }

    /**
     * Checks if a user exists with the provided hashed OAuth provider id.
     */
    public bool existsByProviderId(const string provider, const string id)
    {
        return !findByProviderId(provider, id).isNull();
    }

    /**
     * Finds a user with the provided hashed OAuth provider id.
     */
    public Nullable!User findByProviderId(const string provider, const string id)
    {
        return mongoService.findOne!User(["oauthProviderIds." ~ provider: id]);
    }

    ///
    public bool existsByUsername(const string username)
    {
        return !findByUsername(username).isNull();
    }

    ///
    public Nullable!User findByUsername(const string username)
    {
        return mongoService.findOne!User(["username": username]);
    }

    ///
    public Nullable!User findById(const BsonObjectID id)
    {
        return mongoService.findOneById!User(id);
    }

    /**
     * Inserts a user into the DB. Modifies the provided user's ID field.
     */
    public void createUser(ref User user)
    {
        user.id = BsonObjectID.generate();

        mongoService.insert!User(user);
    }

    /**
     * Updates the provider user's avatar url (the new url should be in the same provided struct).
     */
    public void updateUserAvatarUrl(User user)
    {
        mongoService.update!User(["_id": user.id], ["$set": [User.avatarUrl.stringof: user.avatarUrl]]);
    }

    /**
     * Returns the number of existing users.
     */
    public ulong getUserCount()
    {
        return mongoService.getDocumentCount!User();
    }

    /**
     * Checks if the provided username is valid. Usernames must be unique, can contain alphanumeric chars and these symbols: ., -, _
     *
     * It does check if the username is already taken.
     *
     * Returns: `null` if username is valid, otherwise returns an explanation message.
     */
    public string validateUsername(const string username)
    {
        import std.regex : ctRegex, matchFirst;

        const rgx = ctRegex!(r"^[\w.-]+$");

        if (existsByUsername(username))
            return "Username is already taken.";

        if (username.length < minUsernameLength)
            return "Usernames must be at least " ~ minUsernameLength ~ " characters long.";

        if (username.length > maxUsernameLength)
            return "Username length must be less than or equal to " ~ maxUsernameLength ~ ".";

        if (matchFirst(username, rgx).empty)
            return "Username contains invalid symbols.";

        return null;
    }
}
