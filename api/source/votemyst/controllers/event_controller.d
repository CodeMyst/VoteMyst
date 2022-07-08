module votemyst.controllers.event_controller;

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

    /**
     * GET /api/event/:vanityUrl/artSubmissions
     *
     * Returns all existing art submissions for this event.
     */
    @path("/:vanityUrl/artSubmissions")
    @anyAuth
    const(ArtEntry)[] getArtSubmissions(string _vanityUrl) @safe;

    /**
     * GET /api/event/:vanityUrl/submitted
     *
     * Checks if the current logged in user has submitted to this event.
     */
    @path("/:vanityUrl/submitted")
    @anyAuth
    void getSubmitted(AuthInfo auth, string _vanityUrl) @safe;

    /**
     * POST /api/event/:vanityUrl/:entryId/upvote
     *
     * Upvotes a single entry.
     */
    @path("/:vanityUrl/:entryId/upvote")
    @anyAuth
    void postEntryUpvote(AuthInfo auth, string _vanityUrl, string _entryId) @safe;

    /**
     * GET /api/event/:vanityUrl/:entryId/upvote
     *
     * Checks if the current user upvoted the specified entry.
     */
    @path("/:vanityUrl/:entryId/upvote")
    @anyAuth
    void getEntryUpvote(AuthInfo auth, string _vanityUrl, string _entryId) @safe;

    /**
     * DELETE /api/event/:vanityUrl/:entryId/upvote
     *
     * Removes the current user's upvote on the specified entry.
     */
    @path("/:vanityUrl/:entryId/upvote")
    @anyAuth
    void deleteEntryUpvote(AuthInfo auth, string _vanityUrl, string _entryId) @safe;
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
    private EntryService entryService;

    ///
    public this(AuthService authService, UserService userService,
        EventService eventService, ConfigService configService, EntryService entryService)
    {
        this.authService = authService;
        this.userService = userService;
        this.eventService = eventService;
        this.configService = configService;
        this.entryService = entryService;
    }

    public override Event postEvent(AuthInfo auth, EventCreateInfo createInfo) @safe
    {
        import std.conv : to;
        import std.file : mkdir;
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

        if (createInfo.voteType == VoteType.categories)
        {
            enforceHTTP(createInfo.categories.length > 1, HTTPStatus.badRequest,
                "At least one category must be specified.");
        }

        Event event = {
            vanityUrl: createInfo.vanityUrl,
            title: createInfo.title,
            shortDescription: createInfo.shortDescription,
            description: createInfo.description,
            type: createInfo.type,
            settings: createInfo.settings,
            voteType: createInfo.voteType,
            categories: createInfo.categories,
            revealDate: createInfo.revealDate,
            submissionStartDate: createInfo.submissionStartDate,
            submissionEndDate: createInfo.submissionEndDate,
            voteEndDate: createInfo.voteEndDate,
            hostIds: [auth.id]
        };

        eventService.createEvent(event);

        // create dir to hold all assets
        mkdir("static/events/" ~ event.vanityUrl);

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
        import std.array : array;
        import std.file : copy, remove;
        import std.path : chainPath, extension;

        enforceHTTP(auth.isLoggedIn(), HTTPStatus.unauthorized);

        enforceHTTP(eventService.existsByVanityUrl(_vanityUrl), HTTPStatus.notFound);

        const event = eventService.findEventByVanityUrl(_vanityUrl).get();

        enforceHTTP(event.type == EventType.art, HTTPStatus.badRequest,
            "Trying to post an art submission to a non art event.");

        enforceHTTP(!entryService.existsByEventAndAuthor(event.id, auth.id), HTTPStatus.badRequest,
            "Only one submission per user is allowed for this event.");

        enforceHTTP(Clock.currTime(UTC()) > event.submissionStartDate, HTTPStatus.forbidden,
            "Event submissions are not yet opened.");

        enforceHTTP(Clock.currTime(UTC()) < event.submissionEndDate, HTTPStatus.forbidden,
            "Event submissions are closed.");

        const file = "file" in req.files();

        enforceHTTP(file !is null, HTTPStatus.badRequest, "Missing file.");

        enforceHTTP(validateImage(file.tempPath.toString()), HTTPStatus.badRequest, "Not a valid image file.");

        ArtEntry entry = new ArtEntry();
        entry.id = BsonObjectID.generate();
        entry.eventId = event.id;
        entry.authorId = auth.id;
        entry.submitDate = Clock.currTime(UTC());

        const ext = extension(file.filename.name);
        copy(file.tempPath.toString(),
             chainPath("./static/events/", event.vanityUrl, entry.id.toString() ~ ext).array());
        remove(file.tempPath.toString());

        entry.filename = entry.id.toString() ~ ext;

        entryService.createArtEntry(entry);

        return entry;
    }

    public override const(ArtEntry)[] getArtSubmissions(string _vanityUrl) @safe
    {
        enforceHTTP(eventService.existsByVanityUrl(_vanityUrl), HTTPStatus.notFound);

        const event = eventService.findEventByVanityUrl(_vanityUrl).get();

        enforceHTTP(event.type == EventType.art, HTTPStatus.badRequest,
            "Trying to get art submissions to a non art event.");

        auto entries = entryService.findAllArtEntries(event.id);

        if (event.settings | EventSettings.randomizeEntries)
        {
            import std.random : randomShuffle;

            entries.randomShuffle();
        }

        return entries;
    }

    public override void getSubmitted(AuthInfo auth, string _vanityUrl) @safe
    {
        enforceHTTP(auth.isLoggedIn(), HTTPStatus.unauthorized);

        enforceHTTP(eventService.existsByVanityUrl(_vanityUrl), HTTPStatus.notFound);

        const event = eventService.findEventByVanityUrl(_vanityUrl).get();

        enforceHTTP(entryService.existsByEventAndAuthor(event.id, auth.id), HTTPStatus.notFound);
    }

    public override void postEntryUpvote(AuthInfo auth, string _vanityUrl, string _entryId) @safe
    {
        import std.algorithm : canFind;

        enforceHTTP(auth.isLoggedIn(), HTTPStatus.unauthorized);

        enforceHTTP(eventService.existsByVanityUrl(_vanityUrl), HTTPStatus.notFound);

        const event = eventService.findEventByVanityUrl(_vanityUrl).get();

        enforceHTTP(Clock.currTime(UTC()) > event.submissionEndDate, HTTPStatus.forbidden,
            "Event voting hasn't begun yet.");

        enforceHTTP(Clock.currTime(UTC()) < event.voteEndDate, HTTPStatus.forbidden,
            "Event voting is over.");

        auto entry = entryService.findById(BsonObjectID.fromString(_entryId));

        enforceHTTP(entry !is null, HTTPStatus.notFound);

        enforceHTTP(entry.authorId != auth.id, HTTPStatus.forbidden,
            "You can't upvote your own entry.");

        enforceHTTP(!entry.votes.canFind!(v => v.authorId == auth.id), HTTPStatus.forbidden,
            "You have already voted on this entry.");

        auto vote = new UpvoteVote();
        vote.id = BsonObjectID.generate();
        vote.authorId = auth.id;
        vote.votedAt = Clock.currTime(UTC());

        entry.votes ~= vote;

        entryService.update(entry);
    }

    public override void getEntryUpvote(AuthInfo auth, string _vanityUrl, string _entryId) @safe
    {
        import std.algorithm : canFind;

        enforceHTTP(auth.isLoggedIn(), HTTPStatus.unauthorized);

        enforceHTTP(eventService.existsByVanityUrl(_vanityUrl), HTTPStatus.notFound);

        auto entry = entryService.findById(BsonObjectID.fromString(_entryId));

        enforceHTTP(entry !is null, HTTPStatus.notFound);

        enforceHTTP(entry.votes.canFind!(v => v.authorId == auth.id), HTTPStatus.notFound);
    }

    public override void deleteEntryUpvote(AuthInfo auth, string _vanityUrl, string _entryId) @safe
    {
        import std.algorithm : canFind, remove;

        enforceHTTP(auth.isLoggedIn(), HTTPStatus.unauthorized);

        enforceHTTP(eventService.existsByVanityUrl(_vanityUrl), HTTPStatus.notFound);

        const event = eventService.findEventByVanityUrl(_vanityUrl).get();

        enforceHTTP(Clock.currTime(UTC()) > event.submissionEndDate, HTTPStatus.forbidden,
            "Event voting hasn't begun yet.");

        enforceHTTP(Clock.currTime(UTC()) < event.voteEndDate, HTTPStatus.forbidden,
            "Event voting is over.");

        auto entry = entryService.findById(BsonObjectID.fromString(_entryId));

        enforceHTTP(entry !is null, HTTPStatus.notFound);

        enforceHTTP(entry.votes.canFind!(v => v.authorId == auth.id), HTTPStatus.notFound);

        entry.votes.remove!(v => v.authorId == auth.id);

        entryService.update(entry);
    }

    /**
     * Helper function to return raw `HTTPServerRequest` in REST interfaces because this is the only way to do that.
     *
     * Just passing `HTTPServerRequest` to a REST interface param will throw an error.
     */
    public HTTPServerRequest getReq(HTTPServerRequest req, HTTPServerResponse _) @safe { return req; }
}
