module votemyst.models.user_permissions;

/**
 * List of all permissions users can have on their accounts.
 */
public enum UserPermissions : ulong
{
    none = 0,

    /**
     * Members with this permission are allowed to participate in events hosted on the site.
     */
    participateInEvents = 1uL << 0,

    /**
     * Members with this permission are allowed to edit their profile.
     */
    manageSelf = 1uL << 1,

    /**
     * Members with this permission are allowed to create and host new events.
     */
    createEvents = 1uL << 4,

    /**
     * Members with this permission are allowed to manage events that they aren't the host of.
     */
    manageAllEvents = 1uL << 8,

    /**
     * Members with this permission are allowed to manage and edit other users.
     */
    manageUsers = 1uL << 9,

    defaultPerms = participateInEvents | manageSelf,
    adminPerms = defaultPerms | createEvents | manageAllEvents | manageUsers
}
