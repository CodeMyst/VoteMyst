using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Json;
using System.Text.Json.Serialization;
using VoteMyst.Database;

namespace VoteMyst.OAuth
{
    public class OAuthService
    {
        public Service AssociatedService => System.Enum.Parse<Service>(Identifier, true);

        [JsonPropertyName("identifier")]
        public string Identifier { get; set; }
        [JsonPropertyName("displayName")]
        public string DisplayName { get; set; }
        [JsonPropertyName("domain")]
        public string Domain { get; set; }
        [JsonPropertyName("iconUrl")]
        public string IconUrl { get; set; }

        [JsonPropertyName("clientId")]
        public string ClientId { get; set; }
        [JsonPropertyName("clientSecret")]
        public string ClientSecret { get; set; }

        [JsonPropertyName("authorizationEndpoint")]
        public string AuthorizationEndpoint { get; set; }
        [JsonPropertyName("tokenEndpoint")]
        public string TokenEndpoint { get; set; }
        [JsonPropertyName("userInfoEndpoint")]
        public string UserInfoEndpoint { get; set; }

        [JsonPropertyName("scope")]
        public string Scope { get; set; }
        [JsonPropertyName("nameIdentifierKey")]
        public string NameIdentifierKey { get; set; }
        [JsonPropertyName("nameKey")]
        public string NameKey { get; set; }
        [JsonPropertyName("avatarKey")]
        public string AvatarKey { get; set; }

        public void AddServiceToBuilder(AuthenticationBuilder authBuilder)
        {
            authBuilder.AddOAuth(Identifier, options =>
            {
                options.ClientId = ClientId;
                options.ClientSecret = ClientSecret;
                options.CallbackPath = new PathString($"/signin-{Identifier}");
                options.AccessDeniedPath = new PathString("/");
                
                options.AuthorizationEndpoint = Domain + AuthorizationEndpoint;
                options.TokenEndpoint = Domain + TokenEndpoint;
                options.UserInformationEndpoint = Domain + UserInfoEndpoint;

                options.ClaimActions.MapJsonKey(ClaimTypes.NameIdentifier, NameIdentifierKey);
                options.ClaimActions.MapJsonKey(ClaimTypes.Name, NameKey);
                options.ClaimActions.MapJsonKey("avatar", AvatarKey);

                options.SaveTokens = true;

                options.Events = new OAuthEvents
                {
                    OnCreatingTicket = async context =>
                    {
                        var request = new HttpRequestMessage(HttpMethod.Get, context.Options.UserInformationEndpoint);
                        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", context.AccessToken);

                        var response = await context.Backchannel.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, context.HttpContext.RequestAborted);
                        response.EnsureSuccessStatusCode();

                        var user = await JsonDocument.ParseAsync(await response.Content.ReadAsStreamAsync());

                        context.RunClaimActions(user.RootElement);
                    }
                };

                options.Scope.Add(Scope);
            });
        }
    }
}