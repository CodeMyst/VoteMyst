using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using VoteMyst.Discord;
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

        public UserData FromContext(HttpContext context)
        {
            if (!context.User.Identity.IsAuthenticated)
                return _helpers.Users.Guest();

            string userToken = context.GetTokenAsync(CookieAuthenticationDefaults.AuthenticationScheme, "access_token").GetAwaiter().GetResult();
            if (context.User.Identity.AuthenticationType == "Discord") 
            {
                DiscordService discord = new DiscordService(userToken);
                DiscordUser discordUser = discord.GetUserAsync().GetAwaiter().GetResult();
                
                UserData user = _helpers.Authorization.GetAuthorizedUser(ServiceType.Discord, discordUser.ID.ToString());
                if (user == null) 
                {
                    user = _helpers.Users.NewUser();
                    user.Username = discordUser.Username;

                    // Download the avatar image
                    DownloadHelper.DownloadFile($"https://cdn.discordapp.com/avatars/{discordUser.ID}/{discordUser.Avatar}.png",
                        System.IO.Path.Combine(_environment.WebRootPath, $"assets/avatars/{user.DisplayId}.png"));

                    _helpers.Authorization.AddAuthorizedUser(user.UserId, discordUser.ID.ToString(), ServiceType.Discord);
                }
                return user;
            }
            
            return null;
        }

        public UserData FromId(string displayId)
            => _helpers.Users.GetUser(displayId);
    }
}