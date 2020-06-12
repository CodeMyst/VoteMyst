# VoteMyst

**A simple website to  allow voting on content.**

## Building

Rename the `appsettings.example.json` file to `appsettings.json` and edit the placeholder fields with actual data.

To add the Discord ID and secret run these commands:

```sh
dotnet user-secrets set "Discord:ClientId" "12345"
dotnet user-secrets set "Discord:ClientSecret" "12345"
```

If you are running this in production on a server, and have SSL enabled, set the `VOTEMYST_ENV` variable to `Server` so that the OAUth redirection urls use the proper scheme.

## Contributing

Thank you for your interested in contributing to the project! 

Before you start coding, we ask you to review our [contributing guidelines](./contributing).