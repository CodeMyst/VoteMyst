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

namespace VoteMyst
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRazorPages();
          
            services.AddSingleton(Configuration);
            services.AddMvc(options => options.EnableEndpointRouting = false );

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = "Discord";
            })

            .AddCookie(options =>
            {
                options.ExpireTimeSpan = new TimeSpan (2, 0, 0, 0);
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

            services.AddDbContext<VoteMystContext>(options => options.UseMySql(Configuration["MySQLConnection"]));

            services.AddScoped<UserDataHelper>();
            services.AddScoped<EventHelper>();
            services.AddScoped<EntryHelper>();
            services.AddScoped<VoteHelper>();
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

            // TODO: add https redirection later, but might not even be needed, this should be handled by the web server like nginx
            // app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();
            app.UseAuthentication();

            app.UseEndpoints(endpoints =>
            {
                // Consult the wiki about page information
                
                // Login/Logout Pages
                endpoints.MapControllerRoute(name: "login",
                    pattern: "login",
                    defaults: new { controller = "User", action = "Login" });
                endpoints.MapControllerRoute(name: "logout",
                    pattern: "logout",
                    defaults: new { controller = "User", action = "Logout" });
                    
                // User searching
                endpoints.MapControllerRoute(name: "searchUser",
                    pattern: "users",
                    defaults: new { controller = "User", action = "Search" });
                // Self user viewing (indirectly uses the 'other user' view)
                endpoints.MapControllerRoute(name: "viewSelfUser",
                    pattern: "users/me",
                    defaults: new { controller = "User", action = "DisplaySelf" });
                // View user by ID
                endpoints.MapControllerRoute(name: "viewUser",
                    pattern: "users/{*userId:int}",
                    defaults: new { controller = "User", action = "Display" });

                // Browse events / Create event / Edit event
                endpoints.MapControllerRoute(name: "newEvent",
                    pattern: "events/{action:alpha}",
                    defaults: new { controller = "Event", action = "Browse" });
                // View event by ID
                endpoints.MapControllerRoute(name: "viewEvent",
                    pattern: "events/{eventId:int}",
                    defaults: new { controller = "Event", action = "Display" });

                // Vote on an event
                endpoints.MapControllerRoute(name: "vote",
                    pattern: "vote");
                // Submit an entry to an event
                endpoints.MapControllerRoute(name: "submit",
                    pattern: "submit");

                endpoints.MapControllerRoute(name: "default", 
                    pattern: "{controller=Home}/{action=Index}");
            });
        }
    }
}
