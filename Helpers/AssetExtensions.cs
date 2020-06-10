using System.IO;

using VoteMyst.Database;

using Microsoft.AspNetCore.Hosting;

namespace VoteMyst 
{
    public static class AssetExtensions
    {
        public static string GetRelativeUrl(this Entry entry)
            => $"assets/events/{entry.Event.DisplayID}/{entry.Content}";
        public static string GetAbsoluteUrl(this Entry entry, IWebHostEnvironment environment)
            => Path.Combine(environment.WebRootPath, GetRelativeUrl(entry));
    }
}