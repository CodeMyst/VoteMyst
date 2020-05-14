using System;
using System.IO;

using Microsoft.AspNetCore.Hosting;

using VoteMyst.Database;
using VoteMyst.Database.Models;

namespace VoteMyst
{
    public sealed class PathBuilder
    {
        private readonly IWebHostEnvironment _environment;

        public PathBuilder(IWebHostEnvironment environment)
        {
            _environment = environment;
        }

        public string GetUserAvatarUrl(UserData user)
            => $"/assets/avatars/{user.DisplayId}.png";

        public string GetEntryUrl(Entry entry)
        {
            if (entry.EntryType != EntryType.File)
                throw new InvalidOperationException("Can't get the URL of a non-file entry.");

            return CombineWithWebRoot(entry.Content);
        }
        public string GetEntryUrlAbsolute(Event targetEvent, UserData author, string file)
        {
            return CombineWithWebRoot(GetEntryUrlRelative(targetEvent, author, file));
        }
        public string GetEntryUrlRelative(Event targetEvent, UserData author, string file)
        {
            string extension = Path.GetExtension(file);
            return $"assets/events/{targetEvent.EventId}/{author.DisplayId}{extension}";
        }

        private string CombineWithWebRoot(string path)
            => Path.Combine(_environment.WebRootPath, path);
    }
}