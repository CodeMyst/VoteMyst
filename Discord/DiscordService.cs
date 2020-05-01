using System;
using System.Web;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Collections.Generic;

using Newtonsoft.Json;

namespace VoteMyst.Discord 
{
    public class DiscordService
    {
        public HttpClient Client { get; }

        public DiscordService(HttpClient client)
        {
            client.BaseAddress = new Uri("https://discordapp.com/api/");
            client.DefaultRequestHeaders.Add("Accept", "application/json");
            client.DefaultRequestHeaders.Add("User-Agent", "VoteMyst (vote.myst.rs, 0.1)");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "ZEicD9knmkMWRGFpwZoLMqcsRzq0e9");

            Client = client;
        }

        public async Task<DiscordUser> GetUserAsync()
        {
            var response = await Client.GetAsync($"users/@me");
            response.EnsureSuccessStatusCode();
            string json = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<DiscordUser>(json);
        }
    }
}