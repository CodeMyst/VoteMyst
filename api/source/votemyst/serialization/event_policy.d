module votemyst.serialization.event_policy;

import vibe.d;
import votemyst.models;

/**
 * A specific serialization policy for the Event struct.
 * Used in any API interface that returns the Event object to the caller.
 * It hides the ID from the public.
 */
public template EventPolicy(T) if (is(T : Event))
{
    /**
     * Converts a Event struct to a JSON string without the _id field.
     */
    public static Json toRepresentation(in T data) @safe
    {
        auto json = serializeToJson(data);
        json.remove("_id");

        return json;
    }

    /**
     * Converts a JSON string to a Event struct without any modifications.
     */
    public static T fromRepresentation(in Json json) @safe
    {
        return deserializeJson!T(json);
    }
}
