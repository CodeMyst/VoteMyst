import poodinis;
import vibe.d;
import votemyst.controllers;
import votemyst.services;

void main()
{
    auto configService = new ConfigService("config.json");

    auto dependencies = new shared DependencyContainer();
    dependencies.register!ConfigService().existingInstance(configService);
    dependencies.register!MongoService();
    dependencies.register!UserService();
    dependencies.register!AuthService();
    dependencies.register!EventService();

    dependencies.register!AuthController();
    dependencies.register!AuthWebController();
    dependencies.register!UserController();
    dependencies.register!EventController();

    auto router = new URLRouter();
    router.registerWebInterface(dependencies.resolve!AuthWebController());

    router.registerRestInterface(dependencies.resolve!AuthController());
    router.registerRestInterface(dependencies.resolve!UserController());
    router.registerRestInterface(dependencies.resolve!EventController());

    auto fsettings = new HTTPFileServerSettings();
    fsettings.serverPathPrefix = "/static";

    router.get("/static/*", serveStaticFiles("static/", fsettings));

    auto serverSettings = new HTTPServerSettings();
    serverSettings.bindAddresses = ["127.0.0.1", "localhost"];
    serverSettings.port = configService.port;
    serverSettings.sessionOptions = SessionOption.noSameSiteStrict | SessionOption.httpOnly;
    serverSettings.sessionStore = new MemorySessionStore();

    listenHTTP(serverSettings, router);
    runApplication();
}
