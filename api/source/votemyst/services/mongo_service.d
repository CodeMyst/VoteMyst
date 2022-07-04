module votemyst.services.mongo_service;

import std.typecons;
import vibe.d;
import votemyst.models;
import votemyst.services;

@safe:

/**
 * Service for handling MongoDB stuff.
 */
public class MongoService
{
    private MongoDatabase db;

    ///
    public this(ConfigService config)
    {
        db = connectMongoDB(config.mongoConnectionString).getDatabase(config.mongoDatabase);

        IndexOptions idxOpts;
        idxOpts.unique = true;

        db["users"].createIndex(["username": "text"]);
        db["users"].createIndex(["username": 1], idxOpts);

        db["events"].createIndex(["vanityUrl": "text"]);
        db["events"].createIndex(["vanityUrl": 1], idxOpts);
    }

    /**
     * Resolves a type to a mongo collection name.
     */
    private string getCollectionName(T)() const
    {
        static if (is(T == User))
        {
            return "users";
        }
        else static if (is(T == Event))
        {
            return "events";
        }
        else static if (is(T : Entry))
        {
            return "entries";
        }
        else
        {
            static assert(false, "Cannot get a collection name from the type " ~ T.stringof);
        }
    }

    /**
     * Returns all elements based on the query.
     */
    public MongoCursor!R find(R, T)(T query)
    {
        auto collection = db[getCollectionName!R()];

        return collection.find!R(query);
    }

    /**
     * Finds one element based on the query.
     */
    public auto findOne(R, T)(T query)
    {
        auto collection = db[getCollectionName!R()];

        return collection.findOne!R(query);
    }

    /**
     * Finds one element based on the id. Same as `findOne(["_id": id])`
     */
    public Nullable!R findOneById(R, T)(T id)
    {
        return findOne!R(["_id": id]);
    }

    /**
     * Inserts an element into the DB.
     */
    public void insert(T)(T element)
    {
        auto collection = db[getCollectionName!T()];

        collection.insert(element);
    }

    /**
     * Updates an item in the DB.
     */
    public void update(T, S, U)(S selector, U update)
    {
        auto collection = db[getCollectionName!T()];

        collection.update(selector, update);
    }

    /**
     * Returns the count of document inside the specified collection.
     */
    public ulong getDocumentCount(T)()
    {
        auto collection = db[getCollectionName!T()];

        return collection.find().count();
    }
}
