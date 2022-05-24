import poodinis;
import vibe.d;
import votemyst.services;

void main()
{
    auto configService = new ConfigService("config.json");

    auto dependencies = new shared DependencyContainer();
    dependencies.register!ConfigService().existingInstance(configService);

    auto router = new URLRouter();

    auto serverSettings = new HTTPServerSettings();
    serverSettings.bindAddresses = ["127.0.0.1", "localhost"];
    serverSettings.port = configService.port;
    serverSettings.sessionOptions = SessionOption.noSameSiteStrict | SessionOption.httpOnly;
    serverSettings.sessionStore = new MemorySessionStore();

    listenHTTP(serverSettings, router);
    runApplication();
}
