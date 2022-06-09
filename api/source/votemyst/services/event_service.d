module votemyst.services.event_service;

import std.typecons;
import vibe.d;
import votemyst.models;
import votemyst.services;
import votemyst.utils;

@safe:

///
public class EventService
{
    private MongoService mongoService;

    ///
    public this(MongoService mongoService)
    {
        this.mongoService = mongoService;
    }

    ///
    public bool existsByVanityUrl(const string vanityUrl)
    {
        return !findEventByVanityUrl(vanityUrl).isNull();
    }

    ///
    public Nullable!Event findEventByVanityUrl(const string vanityUrl)
    {
        return mongoService.findOne!Event([Event.vanityUrl.stringof: vanityUrl]);
    }

    /**
     * Inserts an event into the DB. Modifies the ID and ID field.
     */
    public void createEvent(ref Event event)
    {
        import std.string : empty;
        event.id = BsonObjectID.generate();

        if (event.vanityUrl.empty())
        {
            event.vanityUrl = event.id.toString();
        }

        mongoService.insert!Event(event);
    }

    /**
     * Returns all events which are revealed, or the provided user is the host of (can be empty).
     */
    public Event[] findAllRevealed(const BsonObjectID userId)
    {
        import std.datetime : Clock, UTC;

        auto cur = mongoService.find!Event([
            "$or": [
                Bson(["revealDate": Bson(["$lt": Bson(BsonDate(Clock.currTime(UTC())))])]),
                Bson(["hostIds": Bson(userId)])
            ]
        ]);

        Event[] res;
        foreach (iterator; cur)
        {
            res ~= iterator;
        }

        return res;
    }
}
