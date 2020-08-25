using System.IO;
using System.Net;
using System.Security.Claims;

using Microsoft.AspNetCore.Hosting;

using VoteMyst.Database;

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

            string nameIdentifier = principal.FindFirstValue(ClaimTypes.NameIdentifier);
            if (principal.Identity.AuthenticationType == "Discord") 
            {
                UserAccount user = _helpers.Authorization.GetAuthorizedUser(Service.Discord, nameIdentifier);
                if (user == null) 
                {
                    user = _helpers.Users.NewUser(FetchUsername(principal));

                    DownloadAvatar(principal, user);

                    _helpers.Authorization.AddAuthorizedUser(user, nameIdentifier.ToString(), Service.Discord);
                }
                return user;
            }
            
            return null;
        }

        public UserAccount FromId(string displayId)
            => _helpers.Context.QueryByDisplayID<UserAccount>(displayId);

        public void UpdateDataFromService(ClaimsPrincipal principal)
        {
            UserAccount user = FromPrincipal(principal);

            user.Username = FetchUsername(principal);
            DownloadAvatar(principal, user);

            _helpers.Context.UpdateAndSave(user);
        }

        private string FetchUsername(ClaimsPrincipal principal)
            => principal.FindFirstValue(ClaimTypes.Name);
        private void DownloadAvatar(ClaimsPrincipal principal, UserAccount target)
        {
            string avatarHash = principal.FindFirstValue("urn:discord:avatar");
            if (!string.IsNullOrEmpty(avatarHash))
            {
                // Download the avatar image
                string sourceUrl = $"https://cdn.discordapp.com/avatars/{principal.FindFirstValue(ClaimTypes.NameIdentifier)}/{avatarHash}.png";
                string targetPath = Path.Combine(_environment.WebRootPath, $"assets/avatars/{target.DisplayID}.png");

                Directory.CreateDirectory(Path.GetDirectoryName(targetPath));
                using (var client = new WebClient())
                {
                    client.DownloadFile(sourceUrl, targetPath);
                }
            }
        }
    }
}