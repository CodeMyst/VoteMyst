module votemyst.utils.file_utils;

/**
 * Validates that the provided file is an actual image.
 */
public bool validateImage(const string filePath) @safe
{
    import std.process : execute;
    import std.string : indexOf;

    // check file mime using the file cmd
    const filecmd = execute(["file", "-ib", filePath]);

    if (filecmd.status != 0) return false;

    // get the first part of mime (e.g. image/png)
    const slashIdx = filecmd.output.indexOf("/");
    if (slashIdx == -1) return false;
    if (filecmd.output[0 .. slashIdx] != "image") return false;

    // run imagemagick identify on the file
    const magickcmd = execute(["magick", "identify", filePath]);

    if (magickcmd.status != 0) return false;

    return true;
}
