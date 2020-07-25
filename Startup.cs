using System;
using System.Text.Json;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;

using VoteMyst.OAuth;
using VoteMyst.Database;

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

            services.AddAuthenticationServices();

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
