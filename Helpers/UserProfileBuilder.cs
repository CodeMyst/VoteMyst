using System.IO;
using System.Security.Claims;

using Microsoft.AspNetCore.Hosting;

using VoteMyst.Database;
using VoteMyst.Database.Models;

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

        public UserData FromPrincipal(ClaimsPrincipal principal)
        {
            if (!principal.Identity.IsAuthenticated)
                return _helpers.Users.Guest();

            string nameIdentifier = principal.FindFirstValue(ClaimTypes.NameIdentifier);
            if (principal.Identity.AuthenticationType == "Discord") 
            {
                UserData user = _helpers.Authorization.GetAuthorizedUser(ServiceType.Discord, nameIdentifier);
                if (user == null) 
                {
                    user = _helpers.Users.NewUser();
                    user.Username = principal.FindFirstValue(ClaimTypes.Name);

                    string discordAvatar = principal.FindFirstValue("urn:discord:avatar");
                    if (!string.IsNullOrEmpty(discordAvatar))
                    {
                        // Download the avatar image
                        DownloadHelper.DownloadFile($"https://cdn.discordapp.com/avatars/{nameIdentifier}/{discordAvatar}.png",
                            Path.Combine(_environment.WebRootPath, $"assets/avatars/{user.DisplayId}.png"));
                    }

                    _helpers.Authorization.AddAuthorizedUser(user.UserId, nameIdentifier.ToString(), ServiceType.Discord);
                }
                return user;
            }
            
            return null;
        }

        public UserData FromId(string displayId)
            => _helpers.Users.GetUser(displayId);
    }
}