using System;
using System.Linq;
using System.Text.Json;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using VoteMyst.Discord;
using VoteMyst.Database;
using Microsoft.AspNetCore.Authorization;

namespace VoteMyst
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        public IWebHostEnvironment Environment { get; }

        public Startup(IConfiguration configuration, IWebHostEnvironment environment)
        {
            Configuration = configuration;
            Environment = environment;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<SemVer>();

            services.AddRazorPages();
          
            services.AddSingleton(Configuration);
            services.AddRouting(options => options.LowercaseUrls = true );
            services.AddMvc(options => options.EnableEndpointRouting = false );

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = "Discord";
            })

            .AddCookie(options =>
            {
                options.ExpireTimeSpan = new TimeSpan (7, 0, 0, 0);
                options.Cookie = new CookieBuilder ()
                {
                    Name = "DiscordCookie"
                };
            })

            .AddOAuth ("Discord", options =>
            {
                options.ClientId = Configuration["Discord:ClientId"];
                options.ClientSecret = Configuration["Discord:ClientSecret"];
                options.CallbackPath = new PathString("/signin-discord");

                options.AuthorizationEndpoint = "https://discordapp.com/api/oauth2/authorize";
                options.TokenEndpoint = "https://discordapp.com/api/oauth2/token";
                options.UserInformationEndpoint = "https://discordapp.com/api/users/@me";

                options.ClaimActions.MapJsonKey(ClaimTypes.NameIdentifier, "id", ClaimValueTypes.UInteger64);
                options.ClaimActions.MapJsonKey(ClaimTypes.Name, "username", ClaimValueTypes.String);
                options.ClaimActions.MapJsonKey("urn:discord:discriminator", "discriminator", ClaimValueTypes.UInteger32);
                options.ClaimActions.MapJsonKey("urn:discord:avatar", "avatar", ClaimValueTypes.String);
                options.ClaimActions.MapJsonKey("urn:discord:verified", "verified", ClaimValueTypes.Boolean);

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

                options.Scope.Add("identify");
                options.Scope.Add("guilds");
            });

            services.AddDbContext<VoteMystContext>(options => options
                .UseMySql(Configuration["MySQLConnection"])
                .UseLazyLoadingProxies());

            // Avatar Helper is used by the UserDataHelper which gets instantiated by DatabaseHelperProvider
            services.AddScoped<AvatarHelper>();
            services.AddScoped<DatabaseHelperProvider>();
            services.AddScoped<UserProfileBuilder>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseStatusCodePagesWithReExecute("/error", "?code={0}");
            
            app.UseStaticFiles();

            app.UseRouting();

            // add the https scheme to oauth redirect urls
            // adds this only on the server as it has SSL
            if (System.Environment.GetEnvironmentVariable("VOTEMYST_ENV") == "Server")
            {
                app.Use((context, next) =>
                {
                    context.Request.Scheme = "https";
                    return next();
                });
            }

            app.UseAuthorization();
            app.UseAuthentication();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
