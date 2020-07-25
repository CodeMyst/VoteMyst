using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace VoteMyst.OAuth
{
    public static class OAuthExtensions
    {
        public static void AddAuthenticationServices(this IServiceCollection services)
        {
            AuthenticationBuilder builder = services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            })
            .AddCookie(options =>
            {
                options.Cookie = new CookieBuilder
                {
                    Name = "AuthCookie"
                };
            });

            foreach (OAuthService authService in OAuthProvider.Services)
            {
                authService.AddServiceToBuilder(builder);
            }
        }
    }
}