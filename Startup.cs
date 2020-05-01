using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using VoteMyst.Discord;

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
            services.AddHttpClient<DiscordService>();
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

            app.UseEndpoints(endpoints =>
            {
                // Consult the wiki about page information
                
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
