module votemyst.services.event_service;

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
        return !mongoService.findOne!Event(["vanityUrl": vanityUrl]).isNull();
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
}
