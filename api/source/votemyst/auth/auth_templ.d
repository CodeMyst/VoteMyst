module votemyst.auth.auth_templ;

/**
 * Auth mixins that adds the authenticate method.
 */
public mixin template Auth()
{
    ///
    @noRoute
    public AuthInfo authenticate(scope HTTPServerRequest req, scope HTTPServerResponse res) @trusted
    {
        import std.string : startsWith;

        const string authHeader = req.headers.get("Authorization", null);

        if (authHeader is null)
        {
            return AuthInfo.init;
        }

        enforceHTTP(authHeader.startsWith("Bearer "), HTTPStatus.forbidden,
            "Invalid authorization scheme. The token must be provided as a Bearer token.");

        const encodedToken = authHeader["Bearer ".length .. $];

        string id;
        string username;

        try
        {
            auto jwtToken = JwtToken.decode(encodedToken, configService.jwtSecret);

            id = jwtToken.claims.get("id");
            username = jwtToken.claims.get("username");
        }
        catch (Exception e)
        {
            throw new HTTPStatusException(HTTPStatus.forbidden, "Invalid JWT token.");
        }

        const user = userService.findById(BsonObjectID.fromString(id));

        enforceHTTP(!user.isNull(), HTTPStatus.forbidden, "User doesn't exist.");

        return AuthInfo(BsonObjectID.fromString(id), username, user.get().role, true);
    }
}
