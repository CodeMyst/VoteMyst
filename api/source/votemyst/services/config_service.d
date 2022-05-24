module votemyst.services.config_service;

import std.file;
import std.path;
import vibe.data.json;

@safe:

/**
 * Application wide configuration.
 */
public struct Config
{
    /**
     * On which port the app should be hosted on.
     */
    public ushort port;

    /**
     * On what host is the app hosted on.
     */
    public string host;

    /**
     * MongoDB connection string.
     */
    public string mongoConnectionString;

    /**
     * Name of the mongo database to connect to.
     */
    public string mongoDatabase;
}

/**
 * Service that loads the configuration from file and serves it.
 */
public class ConfigService
{
    ///
    public const Config config;

    ///
    public alias config this;

    ///
    public this(string path)
    {
        if (!exists(path)) throw new Exception("Missing " ~ path);

        Json json = parseJsonString(readText(path));

        config = deserializeJson!Config(json);
    }
}
