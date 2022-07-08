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
     * Finds a single entry by its ID.
     */
    public Entry findById(BsonObjectID id)
    {
        return mongoService.findOneById!ArtEntry(id);
    }

    /**
     * Checks if an entry exists by its event and author.
     */
    public bool existsByEventAndAuthor(BsonObjectID eventId, BsonObjectID authorId)
    {
        return mongoService.findOne!Entry([
            "$and": [
                Bson(["eventId": Bson(eventId)]),
                Bson(["authorId": Bson(authorId)])
            ]
        ]) !is null;
    }

    /**
     * Returns all art entries for the specified event.
     */
    public ArtEntry[] findAllArtEntries(BsonObjectID eventId)
    {
        auto cur = mongoService.find!ArtEntry(["eventId": Bson(eventId)]);

        ArtEntry[] res;
        foreach (iterator; cur) res ~= iterator;

        return res;
    }

    /**
     * Updates only the entrie's votes in the DB.
     */
    public void updateVotes(Entry entry)
    {
        mongoService.update!Entry(["_id": Bson(entry.id)], ["$set": Bson(["votes": serializeToBson(entry.votes)])]);
    }
}
