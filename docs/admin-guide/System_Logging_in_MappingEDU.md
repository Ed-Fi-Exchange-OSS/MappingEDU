# System Logging in MappingEDU

MappingEDU uses good old log4net to record system logs which can be
viewed online. This section provides some obvious and less obvious
points about that.

## Viewing the Logs

The logs are available by **User Menu** \> **Admin Settings** \> **Logs
Tab**.

![](../images/22708438/22708437.png)

## Things You Can See with the Logs

The log list and filter work like you'd expect, so we won't go over
that. Rather, we'll list a few factoids about what you can see with the
logs:

* Logins by user
* Creation and deletion of most things, e.g., Data Standards, Mapping
    Projects.
* .NET Exceptions
* Matchmaker details, such as which part of the Matchmaker was used,
    where a selected "Suggestion" was in the stack of suggestions.
* Users accessing key features, such as password resets.

Each event has a user and time associated with it, which can help with
troubleshooting and support calls.

## Admin Guide Contents

Find out more about how to administer MappingEDU responsibly:

* [Admin Feature List](Admin_Feature_List.md)
* [Administer Guest Login Access](Administer_Guest_Login_Access.md)
* [Manage Users](Manage_Users.md)
* [Manage Organizations](Manage_Organizations.md)
* [MappingEDU Swagger API Docs](MappingEDU_Swagger_API_Docs.md)
* [System Logging in MappingEDU](System_Logging_in_MappingEDU.md)