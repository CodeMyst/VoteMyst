module votemyst.controllers.user_controller;

import vibe.d;
import votemyst.models;
import votemyst.serialization;
import votemyst.services;

/**
 * API /api/user
 */
@path("/api/user")
@serializationPolicy!UserPolicy
public interface IUserController
{
    /**
     * GET /api/user/:username
     *
     * Returns the user with the provided username.
     */
    @path("/username/:username")
    const(User) getUserByUsername(string _username) @safe;

    /**
     * GET /api/user/:id
     *
     * Returns the user with the provided ID.
     */
    @path("/id/:id")
    const(User) getUserById(string _id) @safe;
}

/**
 * API /api/user
 */
public class UserController : IUserController
{
    private UserService userService;

    ///
    public this(UserService userService)
    {
        this.userService = userService;
    }

    public override const(User) getUserByUsername(string _username) @safe
    {
        const user = userService.findByUsername(_username);

        if (user.isNull()) throw new HTTPStatusException(HTTPStatus.notFound);

        return user.get();
    }

    public override const(User) getUserById(string _id) @safe
    {
        const user = userService.findById(BsonObjectID.fromString(_id));

        if (user.isNull()) throw new HTTPStatusException(HTTPStatus.notFound);

        return user.get();
    }
}
