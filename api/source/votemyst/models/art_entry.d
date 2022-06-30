module votemyst.models.art_entry;

import votemyst.models;

/**
 * Submission for art events.
 */
public struct ArtEntry
{
    mixin BaseEntryTmpl;

    /**
     * File name of the uploaded image stored on this server.
     *
     * Includes the extension.
     *
     * Format: {random-id}.{extension}
     */
    public string filename;
}

