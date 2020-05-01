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
                //endpoints.MapRazorPages();

                // Consult the wiki about page information
                
                endpoints.MapControllerRoute(name: "selfUser",
                    pattern: "user/me",
                    defaults: new { controller = "User", action = "DisplaySelf" });
                endpoints.MapControllerRoute(name: "user",
                    pattern: "user/{*userId:int}",
                    defaults: new { controller = "User", action = "Display" });
                endpoints.MapControllerRoute(name: "newEvent",
                    pattern: "event/new",
                    defaults: new { controller = "Event", action = "New" });
                endpoints.MapControllerRoute(name: "event",
                    pattern: "event/{*eventId:int}",
                    defaults: new { controller = "Event", action = "Display" });
                endpoints.MapControllerRoute(name: "vote",
                    pattern: "vote");
                endpoints.MapControllerRoute(name: "submit",
                    pattern: "submit");

                endpoints.MapControllerRoute(name: "default", 
                    pattern: "{controller=Home}/{action=Index}");
            });
        }
    }
}
