module votemyst.utils.id_utils;

@safe:

private const string base36Chars = "0123456789abcdefghijklmnopqrstuvwxyz";

/**
 * Generates a random ID.
 */
public string randomId(uint length = 28)
{
    import std.array : appender;
    import std.random : uniform;

    auto apndr = appender!string();

    for (int i = 0; i < length; i++)
    {
        apndr.put(base36Chars[uniform!"[)"(0, base36Chars.length)]);
    }

    return apndr.data();
}

/**
 * Keeps generating a new random ID as long as the predicate `p` is true.
 */
public string randomIdPred(bool delegate(string) @safe p, uint length = 28) @safe
{
    string id;

    do
    {
        id = randomId(length);
    } while (p(id));

    return id;
}
