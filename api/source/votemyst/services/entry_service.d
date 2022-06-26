module votemyst.services.entry_service;

import std.typecons;
import vibe.d;
import votemyst.models;
import votemyst.services;
import votemyst.utils;

@safe:

///
public class EntryService
{
    private MongoService mongoService;

    ///
    public this(MongoService mongoService)
    {
        this.mongoService = mongoService;
    }

    /**
     * Inserts an art entry into the DB. Doesn't modify the ID field.
     */
    public void createArtEntry(ref ArtEntry entry)
    {
        mongoService.insert!ArtEntry(entry);
    }

    /**
     * Checks if an entry exists by its event and author.
     */
    public bool existsByEventAndAuthor(BsonObjectID eventId, BsonObjectID authorId)
    {
        return !mongoService.findOne!BaseEntry([
            "$and": [
                Bson(["eventId": Bson(eventId)]),
                Bson(["authorId": Bson(authorId)])
            ]
        ]).isNull();
    }
}
