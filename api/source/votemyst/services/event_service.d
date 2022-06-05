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
    public bool existsByDisplayId(const string displayId)
    {
        return !mongoService.findOne!Event(["displayId": displayId]).isNull();
    }

    ///
    public bool existsByVanityUrl(const string vanityUrl)
    {
        return !mongoService.findOne!Event(["vanityUrl": vanityUrl]).isNull();
    }

    /**
     * Inserts an event into the DB. Modifies the ID and Display ID fields.
     */
    public void createEvent(ref Event event)
    {
        event.id = BsonObjectID.generate();
        event.displayId = randomIdPred(&existsByDisplayId);

        mongoService.insert!Event(event);
    }
}
