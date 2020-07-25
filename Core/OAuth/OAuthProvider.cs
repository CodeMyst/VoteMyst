using System.Text.Json;
using System.IO;
using VoteMyst.Database;
using System.Linq;

namespace VoteMyst.OAuth
{
    public static class OAuthProvider 
    {
        public static OAuthService[] Services { get; }
        public static Service[] AvailableTypes => Services.Select(s => s.AssociatedService).ToArray();

        static OAuthProvider ()
        {
            string json = File.ReadAllText("oauth.json");
            Services = JsonSerializer.Deserialize<OAuthService[]>(json);
        }

        public static OAuthService GetService(string s)
            => GetService(System.Enum.Parse<Service>(s, true));
        public static OAuthService GetService(Service s)
            => Services.FirstOrDefault(service => service.AssociatedService == s);
    }
}