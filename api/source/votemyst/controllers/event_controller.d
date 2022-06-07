module votemyst.controllers.event_controller;

import hunt.jwt;
import vibe.d;
import votemyst.constants;
import votemyst.models;
import votemyst.serialization;
import votemyst.services;

/**
 * API /api/event
 */
@path("/api/event")
@serializationPolicy!EventPolicy
public interface IEventController
{
    /**
     * POST /api/event/
     *
     * Creates a new event.
     *
     * Params:
     *      authorization = (header) Bearer JWT token.
     *      createInfo = (body) Event create info.
     */
    @path("/")
    @headerParam("authorization", "Authorization")
    @bodyParam("createInfo")
    Event postEvent(string authorization, EventCreateInfo createInfo) @safe;
}

/**
 * API /api/event
 */
public class EventController : IEventController
{
    private AuthService authService;
    private UserService userService;
    private EventService eventService;

    ///
    public this(AuthService authService, UserService userService, EventService eventService)
    {
        this.authService = authService;
        this.userService = userService;
        this.eventService = eventService;
    }

    public override Event postEvent(string authorization, EventCreateInfo createInfo) @safe
    {
        import std.conv : to;
        import std.regex : ctRegex, matchFirst;
        import std.string : empty;

        const tokenRes = authService.decodeToken(authorization);

        enforceHTTP(tokenRes.ok, HTTPStatus.badRequest, tokenRes.error);

        const currentUser = userService.findById(BsonObjectID.fromString(tokenRes.id)).get();

        enforceHTTP(currentUser.role == UserRole.admin, HTTPStatus.forbidden,
            "Only site admins are allowed to host events for now.");

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
            hostIds: [currentUser.id]
        };

        eventService.createEvent(event);

        return event;
    }
}
