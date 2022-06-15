module votemyst.models.art_entry;

import votemyst.models;

/**
 * Submission for art events.
 */
public struct ArtEntry
{
    ///
    public BaseEntry base;

    public alias base this;

    /**
     * File name of the uploaded image stored on this server.
     *
     * Includes the extension.
     *
     * Format: {random-id}.{extension}
     */
    public string filename;
}

