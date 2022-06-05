module votemyst.constants.event_constants;

import std.regex;

///
public const vanityUrlRegex = ctRegex!(r"^[a-zA-Z\d\-]*$");

///
public const vanityUrlMaxLength = 32;

///
public const eventTitleMaxLength = 64;

///
public const eventDescriptionMaxLength = 512;
