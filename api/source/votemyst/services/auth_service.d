module votemyst.services.auth_service;

import hunt.jwt;
import vibe.d;
import votemyst.models;
import votemyst.services;

@safe:

/**
 * Represents a single web service through which users can login using the OAuth flow.
 */
public struct OAuthProvider
{
    /**
     * Provider name.
     */
    public string name;

    ///
    public string clientId;

    ///
    public string clientSecret;

    /**
     * To be redirected to when logging in.
     */
    public string authorizationUrl;

    /**
     * Url from which an access token can be fetched.
     */
    public string accessTokenUrl;

    /**
     * Url from which user information can be fetched.
     */
    public string userInfoUrl;

    /**
     * Url to which the redirect will be made after authorization.
     */
    public string redirectUrl;

    /**
     * List of scopes that specify account access.
     */
    public string[] scopes;

    /**
     * Field name for the user ID. Used when reading user info from the service.
     */
    public string idJsonField;

    /**
     * Field name for the username. Used when reading user info from the service.
     */
    public string usernameJsonField;

    /**
     * Field name for the user avatar url. Used when reading user info from the service.
     */
    public string avatarUrlJsonField;
}

/**
 * Result of decoding the authorization token.
 */
public struct AuthDecodeRes
{
    ///
    public bool ok;

    ///
    public string error = null;

    ///
    public string id = null;
    ///
    public string username = null;
}

/**
 * Services used for handling anything OAuth related.
 */
public class AuthService
{
    private ConfigService configService;

    ///
    public const OAuthProvider githubProvider;

    ///
    public const OAuthProvider discordProvider;

    ///
    public this(ConfigService config)
    {
        this.configService = config;

        githubProvider = OAuthProvider("GitHub", config.github.clientId, config.github.clientSecret,
            "https://github.com/login/oauth/authorize", "https://github.com/login/oauth/access_token",
            "https://api.github.com/user", config.host ~ "api/auth-web/login/github/callback",
            ["read:user"], "id", "login", "avatar_url");

        discordProvider = OAuthProvider("Discord", config.discord.clientId, config.discord.clientSecret,
            "https://discord.com/api/oauth2/authorize", "https://discord.com/api/oauth2/token",
            "https://discord.com/api/users/@me", config.host ~ "api/auth-web/login/discord/callback",
            ["identify"], "id", "username", "avatar");
    }

    /**
     * Returns the full authorization url for the provider.
     */
    public string getAuthorizationUrl(const OAuthProvider provider, string state) const
    {
        import std.array : join;
        import std.uri : encodeComponent;

        const scopes = provider.scopes.join(",");

        return provider.authorizationUrl ~
            "?client_id=" ~ provider.clientId ~
            "&scope=" ~ scopes ~
            "&redirect_uri=" ~ encodeComponent(provider.redirectUrl) ~
            "&state=" ~ state ~
            "&response_type=code";
    }

    /**
     * Returns the access token from the provided code.
     */
    public string getAccessToken(const OAuthProvider provider, string code) const
    {
        import std.uri : encode;

        string accessToken;

        requestHTTP(provider.accessTokenUrl,
            (scope req)
            {
                req.method = HTTPMethod.POST;

                auto data = ["client_id": provider.clientId,
                             "client_secret": provider.clientSecret,
                             "code": code,
                             "grant_type": "authorization_code",
                             "redirect_uri": encode(provider.redirectUrl)];

                if (provider == discordProvider)
                {
                    req.headers.addField("Accept", "application/x-www-form-urlencoded");
                    req.headers.addField("Content-Type", "application/x-www-form-urlencoded");
                    req.writeFormBody(data);
                }
                else
                {
                    req.headers.addField("Accept", "application/json");
                    req.writeJsonBody(data);
                }
            },
            (scope res)
            {
                if (res.statusCode != HTTPStatus.ok)
                {
                    logError("Failed reading the access token. Error while making a request to %s. Got response: %d.",
                        provider.name, res.statusCode);
                    logError(res.bodyReader.readAllUTF8());
                    throw new HTTPStatusException(HTTPStatus.internalServerError, "Failed reading the access token.");
                }

                try
                {
                    accessToken = parseJsonString(res.bodyReader.readAllUTF8())["access_token"].get!string();
                }
                catch (Exception e)
                {
                    logError("Failed reading the access token. Exception while parsing response JSON from %s.\n%s.",
                        provider.name, e);
                    throw new HTTPStatusException(HTTPStatus.internalServerError, "Failed reading the access token.");
                }
            });

        if (accessToken == "")
        {
            logError("Failed reading the access token. The resulting access token string is empty.");
            throw new HTTPStatusException(HTTPStatus.internalServerError, "Failed reading the access token.");
        }

        return accessToken;
    }

    /**
     * Returns the OAUth provider user from the provided access token.
     */
    public ProviderUser getProviderUser(const OAuthProvider provider,  const string accessToken) const
    {
        import std.conv : to;

        ProviderUser user;

        requestHTTP(provider.userInfoUrl,
            (scope req)
            {
                if (provider == githubProvider)
                {
                    req.headers.addField("Accept", "application/vnd.github.v3+json");
                    req.headers.addField("Authorization", "token " ~ accessToken);
                }
                else
                {
                    req.headers.addField("Accept", "application/json");
                    req.headers.addField("Authorization", "Bearer " ~ accessToken);
                }
            },
            (scope res)
            {
                if (res.statusCode != HTTPStatus.ok)
                {
                    logError("Failed getting the %s user. Response status: %d.",
                        provider.name, res.statusCode);
                    throw new HTTPStatusException(HTTPStatus.internalServerError,
                        "Failed getting the " ~ provider.name ~ " user.");
                }

                try
                {
                    Json json = parseJsonString(res.bodyReader.readAllUTF8());

                    user.id = json[provider.idJsonField].to!string();
                    user.username = json[provider.usernameJsonField].to!string();
                    user.avatarUrl = json[provider.avatarUrlJsonField].to!string();
                }
                catch (Exception e)
                {
                    logError("Failed getting the %s user. Exception: %s.",
                        provider.name, e);
                    throw new HTTPStatusException(HTTPStatus.internalServerError,
                        "Failed getting the " ~ provider.name ~ " user.");
                }
            });

        if (user == ProviderUser.init)
        {
            logError("Failed getting the %s user. The provider user is empty.",
                provider.name);
            throw new HTTPStatusException(HTTPStatus.internalServerError,
                "Failed getting the " ~ provider.name ~ " user.");
        }

        return user;
    }

    /**
     * Validates the provided authorization token.
     *
     * Returns null if token valid, otherwise returns the error message.
     */
    public AuthDecodeRes decodeToken(string token) @trusted
    {
        if (!token.startsWith("Bearer "))
        {
            AuthDecodeRes res =
            {
                ok: false,
                error: "Invalid authorization scheme. The token must be provided as a Bearer token."
            };

            return res;
        }

        const encodedToken = token["Bearer ".length .. $];

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
            AuthDecodeRes res =
            {
                ok: false,
                error: "Provided token is not valid."
            };

            return res;
        }

        AuthDecodeRes res =
        {
            ok: true,
            id: id,
            username: username
        };

        return res;
    }
}
