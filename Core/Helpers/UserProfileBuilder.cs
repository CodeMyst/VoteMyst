using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Claims;

using Microsoft.AspNetCore.Hosting;

using VoteMyst.Database;
using VoteMyst.OAuth;

namespace VoteMyst
{
    public class UserProfileBuilder
    {
        private readonly DatabaseHelperProvider _helpers;
        private readonly IWebHostEnvironment _environment;

        public UserProfileBuilder(DatabaseHelperProvider helpers, IWebHostEnvironment environment)
        {
            _helpers = helpers;
            _environment = environment;
        }

        public UserAccount FromPrincipal(ClaimsPrincipal principal)
        {
            if (!principal.Identity.IsAuthenticated)
                return _helpers.Users.Guest();

            // Find the required claim values
            string nameIdentifier = principal.FindFirstValue(ClaimTypes.NameIdentifier);
            string name = principal.FindFirstValue(ClaimTypes.Name);
            string avatar = principal.FindFirstValue("avatar");

            // Retrieve the appropriate authentication service via the AuthenticationType
            OAuthService authService = OAuthProvider.GetService(principal.Identity.AuthenticationType);
            if (authService == null)
                throw new InvalidOperationException($"Invalid authentication type. Valid types are {string.Join(", ", OAuthProvider.AvailableTypes)}.");

            UserAccount user = _helpers.Authorization.GetAuthorizedUser(authService.AssociatedService, nameIdentifier);
            if (user == null)
            {
                // A user account does not exist yet; we need to create one
                user = _helpers.Users.NewUser(name);

                if (!string.IsNullOrEmpty(avatar)) 
                {
                    // Locate the avatar URL
                    string avatarUrl = null;
                    string targetPath = Path.Combine(_environment.WebRootPath, $"assets/avatars/{user.DisplayID}.png");

                    switch (authService.AssociatedService)
                    {
                        case Service.Discord:
                            // The source URL is a combination of the user ID and the avatar hash.
                            // Source: https://discord.com/developers/docs/reference#image-formatting
                            avatarUrl = $"https://cdn.discordapp.com/avatars/{nameIdentifier}/{avatar}.png";
                            break;
                        case Service.Itch:
                            avatarUrl = avatar;
                            break;
                    }

                    Console.WriteLine(avatarUrl);

                    // Ensure a directory for avatars exists
                    Directory.CreateDirectory(Path.GetDirectoryName(targetPath));
                    using var client = new WebClient();
                    client.DownloadFile(avatarUrl, targetPath);
                }

                _helpers.Authorization.AddAuthorizedUser(user, nameIdentifier, authService.AssociatedService);
            }

            return user;
        }

        public UserAccount FromId(string displayId)
            => _helpers.Context.QueryByDisplayID<UserAccount>(displayId);
    }
}