module votemyst.controllers.auth_controller;

import jwt.algorithms;
import jwt.jwt;
import std.digest.sha;
import vibe.d;
import vibe.web.auth;
import votemyst.auth;
import votemyst.models;
import votemyst.services;

/**
 * API /api/auth
 */
@path("/api/auth")
@requiresAuth
public interface IAuthController
{
    /**
     * POST /api/auth/register
     *
     * Creates a new account.
     *
     * Params:
     *      authorization = (header) Bearer JWT token.
     *      username = (body) Username to be used for the new account.
     */
    @noAuth
    Json postRegister(@viaHeader("authorization") string authorization, @viaBody("username") string username) @safe;

    /**
     * POST /api/auth/self
     *
     * Returns the currently authorized user from the provided token.
     */
    @auth(Role.loggedIn)
    const(User) getSelf(AuthInfo auth) @safe;
}

/**
 * API /api/auth
 */
public class AuthController : IAuthController
{
    mixin Auth;

    private ConfigService configService;
    private UserService userService;
    private AuthService authService;

    ///
    public this(ConfigService configService, UserService userService, AuthService authService)
    {
        this.configService = configService;
        this.userService = userService;
        this.authService = authService;
    }

    public override Json postRegister(string authorization, string username) @trusted
    {
        enforceHTTP(authorization.startsWith("Bearer "), HTTPStatus.badRequest,
            "Invalid authorization scheme. The token must be provided as a Bearer token.");

        const usernameErr = userService.validateUsername(username);

        enforceHTTP(usernameErr is null, HTTPStatus.badRequest, usernameErr);

        const encodedToken = authorization["Bearer ".length .. $];

        string providerName;
        string providerId;
        string avatarUrl;

        try
        {
            auto token = verify(encodedToken, configService.jwtSecret, [JWTAlgorithm.HS512]);

            providerName = token.claims.get("provider");
            providerId = token.claims.get("id");
            avatarUrl = token.claims.get("avatarUrl");
        }
        catch (Exception e)
        {
            logError("User tried to register with an invalid token. Exception: %s", e);
            throw new HTTPStatusException(HTTPStatus.badRequest, "Provided token is not valid.");
        }

        enforceHTTP(!userService.existsByProviderId(providerName, providerId), HTTPStatus.badRequest,
            "A user already exists with the same provider.");

        User user = {
            username: username,
            oauthProviderIds: [providerName: sha256Of(providerId).toHexString()],
            avatarUrl: avatarUrl,
            joinDate: Clock.currTime(),
            // first created account is an admin automatically
            role: userService.getUserCount() == 0 ? UserRole.admin : UserRole.user
        };

        userService.createUser(user);

        // To get the Discord user's avatar, their Discord ID is required.
        // And since their ID is hashed, we have to download the avatar and save it locally.
        if (providerName == authService.discordProvider.name)
        {
            import vibe.inet.urltransfer : download;

            const url = "https://cdn.discordapp.com/avatars/" ~ providerId ~ "/" ~ avatarUrl ~ ".png";

            download(url, "static/avatars/" ~ user.id.toString() ~ ".png");

            user.avatarUrl = configService.host ~ "static/avatars/" ~ user.id.toString() ~ ".png";
            userService.updateUserAvatarUrl(user);
        }

        const timeInMonth = Clock.currTime() + 30.days;
        auto jwtToken = new Token(JWTAlgorithm.HS512);
        jwtToken.claims.exp = timeInMonth.toUnixTime();
        jwtToken.claims.set("id", user.id.toString());
        jwtToken.claims.set("username", user.username);

        return Json(["token": Json(jwtToken.encode(configService.jwtSecret))]);
    }

    public override const(User) getSelf(AuthInfo auth) @trusted
    {
        const user = userService.findById(auth.id);

        return user.get();
    }
}

/**
 * A web controller, not API. Responsible for initializing OAuth provider login.
 */
@path("/api/auth-web")
public class AuthWebController
{
    private const AuthService authService;
    private UserService userService;
    private ConfigService configService;

    ///
    public this(AuthService authService, UserService userService, ConfigService configService)
    {
        this.authService = authService;
        this.userService = userService;
        this.configService = configService;
    }

    /**
     * /api/auth-web/login/:serviceName
     *
     * Initiates logging in with an OAuth provider.
     */
    @path("login/:serviceName")
    public void getOAuthLogin(string _serviceName, HTTPServerResponse res) @safe
    {
        import std.algorithm : filter, map;
        import std.array : appender;
        import std.ascii : isAlphaNum;
        import std.base64 : Base64;
        import std.random : rndGen;
        import std.range : take;

        // generate a random state string for oauth
        auto rndNums = rndGen().map!(a => cast(ubyte) a)().take(32);
        auto apndr = appender!string();
        Base64.encode(rndNums, apndr);
        auto state = apndr.data.filter!isAlphaNum().to!string();

        auto session = res.startSession();
        session.set("oauth_state", state);

        switch (_serviceName)
        {
        case "github":
            {
                res.redirect(authService.getAuthorizationUrl(authService.githubProvider, state));
            }
            break;

        case "discord":
            {
                res.redirect(authService.getAuthorizationUrl(authService.discordProvider, state));
            }
            break;

        default:
            throw new HTTPStatusException(HTTPStatus.badRequest);
        }

        res.redirect(authService.getAuthorizationUrl(authService.githubProvider, state));
    }

    /**
     * This is an OAauth callback from a provider.
     */
    @path("/login/:serviceName/callback")
    public void getGithubCallback(string _serviceName,
        HTTPServerRequest req, HTTPServerResponse res, string code, string state) @trusted
    {
        if (!req.session)
        {
            throw new HTTPStatusException(HTTPStatus.badRequest,
                "OAuth callback called but the session hasn't been started.");
        }

        const sessionState = req.session.get!string("oauth_state");

        if (state != sessionState)
            throw new HTTPStatusException(HTTPStatus.badRequest, "Invalid state code.");

        OAuthProvider provider;

        switch (_serviceName)
        {
        case "github":
            {
                provider = cast(OAuthProvider) authService.githubProvider;
            }
            break;

        case "discord":
            {
                provider = cast(OAuthProvider) authService.discordProvider;
            }
            break;

            default:
                throw new HTTPStatusException(HTTPStatus.badRequest);
        }

        const accessToken = authService.getAccessToken(provider, code);

        const providerUser = authService.getProviderUser(provider, accessToken);

        auto user = userService.findByProviderId(provider.name,
            sha256Of(providerUser.id).toHexString());

        auto jwtToken = new Token(JWTAlgorithm.HS512);

        // todo: make sure cookie is secure on https
        auto cookie = new Cookie();
        cookie.path = "/";
        cookie.httpOnly = false;
        cookie.sameSite(Cookie.SameSite.strict);

        // terminate the session that was only used for storing the OAuth state string
        res.terminateSession();

        // if user doesn't exist, create a jwt token that is used for registration purposes only
        if (user.isNull())
        {
            const timeInHour = Clock.currTime() + 1.hours;
            jwtToken.claims.exp = timeInHour.toUnixTime();

            jwtToken.claims.set("id", providerUser.id);
            jwtToken.claims.set("provider", provider.name);
            jwtToken.claims.set("avatarUrl", providerUser.avatarUrl);

            cookie.expire = dur!"hours"(1);
            cookie.value = jwtToken.encode(configService.jwtSecret);

            res.cookies.addField("votemyst-registration", cookie);

            res.redirect(
                configService.clientHost ~ "create-account?username=" ~ providerUser.username);
        }
        else
        {
            const timeInMonth = Clock.currTime() + 30.days;
            jwtToken.claims.exp = timeInMonth.toUnixTime();
            jwtToken.claims.set("id", user.get().id.toString());
            jwtToken.claims.set("username", user.get().username);

            cookie.expire = dur!"days"(30);
            cookie.value = jwtToken.encode(configService.jwtSecret);

            res.cookies.addField("votemyst", cookie);

            res.redirect(configService.clientHost ~ "handle-login");
        }
    }
}
