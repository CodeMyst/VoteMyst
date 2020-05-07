using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Hosting;
using VoteMyst.Database.Models;

namespace VoteMyst
{
    public class AvatarHelper
    {
        private readonly IWebHostEnvironment _environment;

        public AvatarHelper(IWebHostEnvironment environment)
        {
            _environment = environment;
        }

        public string GetRelativeAvatarUrl(UserData user, out string initials)
        {
            initials = string.Concat(user.Username.Where(c => c >= 'A' && c <= 'Z').Take(2));

            string relativeAvatarUrl = $"assets/avatars/{user.DisplayId}.png";
            bool hasAvatar = File.Exists(GetAbsoluteAvatarUrl(relativeAvatarUrl));
            
            return hasAvatar ? "/" + relativeAvatarUrl : null;
        }

        public string GetAbsoluteAvatarUrl(UserData user)
            => GetAbsoluteAvatarUrl($"assets/avatars/{user.DisplayId}.png");

        private string GetAbsoluteAvatarUrl(string relativeUrl) 
            => Path.Combine(_environment.WebRootPath, relativeUrl);
    }    
}