using System;
using System.Web;
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;

using System.Text.Json;

namespace VoteMyst.Discord 
{
    public class DiscordService
    {
        public HttpClient Client { get; }

        public DiscordService(HttpClient client)
        {
            client.BaseAddress = new Uri("https://discordapp.com/api/v6");
            // GitHub API versioning
            client.DefaultRequestHeaders.Add("Accept",
                "application/json");
            // GitHub requires a user-agent
            client.DefaultRequestHeaders.Add("User-Agent",
                "VoteMyst v0.1");

            Client = client;
        }

        public async Task<DiscordUser> GetUserAsync(ulong userId)
        {
            var response = await Client.GetAsync($"/users/{userId}");
            response.EnsureSuccessStatusCode();
            using var responseStream = await response.Content.ReadAsStreamAsync();
            return await JsonSerializer.DeserializeAsync<DiscordUser>(responseStream);
        }
    }
}