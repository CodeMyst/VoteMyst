module votemyst.controllers.event_controller;

import hunt.jwt;
import vibe.d;
import vibe.web.auth;
import votemyst.auth;
import votemyst.constants;
import votemyst.models;
import votemyst.serialization;
import votemyst.services;
import votemyst.utils;

/**
 * API /api/event
 */
@path("/api/event")
@requiresAuth
public interface IEventController
{
    /**
     * POST /api/event/
     *
     * Creates a new event.
     *
     * Params:
     *      createInfo = (body) Event create info.
     */
    @path("/")
    @auth(Role.admin)
    Event postEvent(AuthInfo auth, @viaBody("") EventCreateInfo createInfo) @safe;

    /**
     * GET /api/event/listing
     *
     * Gets the list of all events.
     */
    @path("/listing")
    @anyAuth
    Event[] getEventListing(AuthInfo auth) @safe;

    /**
     * GET /api/event/:vanityUrl
     *
     * Gets a single event.
     */
    @path("/:vanityUrl")
    @anyAuth
    const(Event) getEvent(AuthInfo auth, string _vanityUrl) @safe;

    /**
     * POST /api/event/:vanityUrl/artSubmit
     *
     * Posts a single submission to an **art** event.
     */
    @path("/:vanityUrl/artSubmit")
    @anyAuth
    const(ArtEntry) postArtSubmission(AuthInfo auth, string _vanityUrl, HTTPServerRequest req) @safe;
}

/**
 * API /api/event
 */
public class EventController : IEventController
{
    mixin Auth;

    private AuthService authService;
    private UserService userService;
    private EventService eventService;
    private ConfigService configService;

    ///
    public this(AuthService authService, UserService userService,
        EventService eventService, ConfigService configService)
    {
        this.authService = authService;
        this.userService = userService;
        this.eventService = eventService;
        this.configService = configService;
    }

    public override Event postEvent(AuthInfo auth, EventCreateInfo createInfo) @safe
    {
        import std.conv : to;
        import std.regex : ctRegex, matchFirst;
        import std.string : empty;

        if (!createInfo.vanityUrl.empty())
        {
            const rgx = ctRegex!(r"^[a-zA-Z\d\-]*$");

            enforceHTTP(!matchFirst(createInfo.vanityUrl, rgx).empty, HTTPStatus.badRequest,
                "The vanity URL may only contain lowercase letters, digits and dashes.");

            enforceHTTP(createInfo.vanityUrl.length < vanityUrlMaxLength, HTTPStatus.badRequest,
                "The vanity URL length must be less than " ~ vanityUrlMaxLength.to!string() ~ " characters.");

            enforceHTTP(!eventService.existsByVanityUrl(createInfo.vanityUrl), HTTPStatus.badRequest,
                "The vanity URL is already taken.");
        }

        enforceHTTP(createInfo.title.length < eventTitleMaxLength, HTTPStatus.badRequest,
            "The event title length must be less than " ~ eventTitleMaxLength.to!string() ~ " characters.");

        enforceHTTP(createInfo.shortDescription.length < eventShortDescriptionMaxLength, HTTPStatus.badRequest,
            "The event short description length must be less than "
            ~ eventShortDescriptionMaxLength.to!string() ~ " characters.");

        enforceHTTP(createInfo.description.length < eventDescriptionMaxLength, HTTPStatus.badRequest,
            "The event description length must be less than " ~ eventDescriptionMaxLength.to!string() ~ " characters.");

        enforceHTTP(createInfo.revealDate < createInfo.submissionStartDate &&
                    createInfo.submissionStartDate < createInfo.submissionEndDate &&
                    createInfo.submissionEndDate < createInfo.voteEndDate,
                    HTTPStatus.badRequest,
                    "All dates must be in ascending order.");

        Event event = {
            vanityUrl: createInfo.vanityUrl,
            title: createInfo.title,
            shortDescription: createInfo.shortDescription,
            description: createInfo.description,
            type: createInfo.type,
            settings: createInfo.settings,
            revealDate: createInfo.revealDate,
            submissionStartDate: createInfo.submissionStartDate,
            submissionEndDate: createInfo.submissionEndDate,
            voteEndDate: createInfo.voteEndDate,
            hostIds: [auth.id]
        };

        eventService.createEvent(event);

        return event;
    }

    public override Event[] getEventListing(AuthInfo auth) @safe
    {
        const currentUserId = auth.isLoggedIn() ? auth.id : BsonObjectID.init;

        return eventService.findAllRevealed(currentUserId);
    }

    public override const(Event) getEvent(AuthInfo auth, string _vanityUrl) @safe
    {
        import std.algorithm : canFind;
        import std.datetime : Clock, UTC;

        const event = eventService.findEventByVanityUrl(_vanityUrl);

        enforceHTTP(!event.isNull(), HTTPStatus.notFound);

        // not yet revealed, check if we are the host
        if (Clock.currTime(UTC()) < event.get().revealDate)
        {
            // don't leak event by returning forbidden status
            enforceHTTP(auth.isLoggedIn(), HTTPStatus.notFound);

            enforceHTTP(event.get().hostIds.canFind(auth.id), HTTPStatus.notFound);
        }

        return event.get();
    }

    @before!getReq("req") // needed to get the raw request to get access to uploaded file
    public override const(ArtEntry) postArtSubmission(AuthInfo auth, string _vanityUrl, HTTPServerRequest req) @safe
    {
        enforceHTTP(eventService.existsByVanityUrl(_vanityUrl), HTTPStatus.notFound);

        const event = eventService.findEventByVanityUrl(_vanityUrl).get();

        enforceHTTP(Clock.currTime(UTC()) > event.submissionStartDate, HTTPStatus.forbidden,
            "Event submissions are not yet opened.");

        enforceHTTP(Clock.currTime(UTC()) < event.submissionEndDate, HTTPStatus.forbidden,
            "Event submissions are closed.");

        const file = "file" in req.files();

        enforceHTTP(file !is null, HTTPStatus.badRequest, "Missing file.");

        enforceHTTP(validateImage(file.tempPath.toString()), HTTPStatus.badRequest, "Not a valid image file.");

        return ArtEntry.init;
    }

    /**
     * Helper function to return raw `HTTPServerRequest` in REST interfaces because this is the only way to do that.
     *
     * Just passing `HTTPServerRequest` to a REST interface param will throw an error.
     */
    public HTTPServerRequest getReq(HTTPServerRequest req, HTTPServerResponse _) @safe { return req; }
}
