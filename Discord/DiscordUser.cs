using System.Text.Json;
using System.Text.Json.Serialization;

namespace VoteMyst.Discord
{
    public class DiscordUser
    {
        [JsonPropertyName("id")]
        public ulong ID { get; set; }
        [JsonPropertyName("username")]
        public string Username { get; set; }
        [JsonPropertyName("discriminator")]
        public string Discriminator { get; set; }
        [JsonPropertyName("avatar")]
        public string Avatar { get; set; }
        [JsonPropertyName("bot")]
        public bool IsBot { get; set; }
    }
}