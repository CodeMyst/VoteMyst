# VoteMyst

A simple website to allow voting on content.

## Building

Rename the `appsettings.example.json` file to `appsettings.json` and edit the placeholder fields with actual data.

To add the Discord ID used for the OAuth process you can use the following commands:

```sh
dotnet user-secrets set "Discord:ClientId" "12345"
dotnet user-secrets set "Discord:ClientSecret" "12345"
```

If you are running this in production on a server, and have SSL enabled, set the `VOTEMYST_ENV` variable to `Server` so that the OAuth redirection URLs use the proper scheme.

VoteMyst uses EntityFramework Migrations to provide the database structure. To make sure your database is up-to-date run `dotnet ef database update`.

To run the project on localhost, simply execute `dotnet run` or `dotnet watch run`.

## Contributing

Thank you for your interested in contributing to the project!

Before you start coding, we ask you to review our [contributing guidelines](./.github/contributing).

If you have a suggestion, bug report or discussion, please open an issue. We provide issue templates for the most common inquiries.

## License

VoteMyst is licensed under a [GNU General Public License](./LICENSE).
