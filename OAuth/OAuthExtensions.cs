using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace VoteMyst.OAuth
{
    public static class OAuthExtensions
    {
        public static void AddAllOAuthServices(this AuthenticationBuilder auth, IConfiguration configuration)
        {
            var configServices = configuration.GetSection("OAuthServices").Get<OAuthService[]>();

            foreach (var s in configServices)
            {
                auth.AddCookie(options =>
                {
                    options.ExpireTimeSpan = new TimeSpan(2, 0, 0, 0);
                    options.Cookie = new CookieBuilder
                    {
                        Name = s.Name + "Cookie"
                    };
                });

                auth.AddOAuth(s.Name, options =>
                {
                    options.ClientId = configuration[s.Name + ":ClientId"];
                    options.ClientSecret = configuration[s.Name + ":ClientSecret"];
                    options.CallbackPath = new PathString("/signin-" + s.Name.ToLower());
                
                    options.AuthorizationEndpoint = s.AuthEndpoint;
                    options.TokenEndpoint = s.TokenEndpoint;
                    options.UserInformationEndpoint = s.UserEndpoint;

                    foreach (OAuthClaim claim in s.Claims)
                    {
                        FieldInfo cti = typeof(ClaimTypes).GetField(claim.Key);
                        
                        string key = cti?.GetValue(null) as string ?? claim.Key;

                        FieldInfo cvti = typeof(ClaimValueTypes).GetField(claim.Type);

                        string type = cvti.GetValue(null) as string;

                        options.ClaimActions.MapJsonKey(key, claim.JsonKey, type);
                    }

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

                    foreach (string scope in s.Scopes)
                    {
                        options.Scope.Add(scope);
                    }
                });
            }
        }
    }
}