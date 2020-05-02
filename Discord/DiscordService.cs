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
        private readonly HttpClient _client;

        public DiscordService(string oauthToken)
        {
            _client = new HttpClient();

            _client.BaseAddress = new Uri("https://discordapp.com/api/");
            _client.DefaultRequestHeaders.Add("Accept", "application/json");
            _client.DefaultRequestHeaders.Add("User-Agent", "VoteMyst (vote.myst.rs, 0.1)");
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", oauthToken);
        }

        public async Task<DiscordUser> GetUserAsync()
        {
            var response = await _client.GetAsync($"users/@me");
            response.EnsureSuccessStatusCode();
            string json = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<DiscordUser>(json);
        }
    }
}