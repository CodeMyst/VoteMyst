module votemyst.models.art_entry;

import votemyst.models;

/**
 * Submission for art events.
 */
public class ArtEntry : Entry
{
    /**
     * File name of the uploaded image stored on this server.
     *
     * Includes the extension.
     *
     * Format: {random-id}.{extension}
     */
    public string filename;
}

