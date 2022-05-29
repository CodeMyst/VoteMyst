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
    @path("/:username")
    const(User) getUser(string _username) @safe;
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

    public override const(User) getUser(string _username) @safe
    {
        const user = userService.findByUsername(_username);

        if (user.isNull()) throw new HTTPStatusException(HTTPStatus.notFound);

        return user.get();
    }
}
