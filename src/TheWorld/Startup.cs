using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNet.Authentication.Cookies;
using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Hosting;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.PlatformAbstractions;
using Newtonsoft.Json.Serialization;
using TheWorld.Caching;
using TheWorld.Models;
using TheWorld.Services;
using TheWorld.ViewModels;

namespace TheWorld
{
    public class Startup
    {
        public static IConfigurationRoot Configuration;

        public Startup(IApplicationEnvironment appEnv, IRuntimeEnvironment runtimeEnvironment, IHostingEnvironment hostingEnvironment)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(appEnv.ApplicationBasePath)
                .AddJsonFile("config.json")
                .AddJsonFile($"config.{hostingEnvironment.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();

            Configuration = builder.Build();
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc()
            .AddJsonOptions(opt =>
            {
                opt.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            });

            services.AddIdentity<WorldUser, IdentityRole>(config =>
            {
                config.User.RequireUniqueEmail = true;
                config.Password.RequiredLength = 8;
                config.Cookies.ApplicationCookie.LoginPath = "/auth/login";
                config.Cookies.ApplicationCookie.Events = new CookieAuthenticationEvents
                {
                    OnRedirectToLogin = ctx =>
                    {
                        if (ctx.Request.Path.StartsWithSegments("/api") &&
                            ctx.Response.StatusCode == (int)HttpStatusCode.OK)
                        {
                            ctx.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                        }
                        else
                        {
                            ctx.Response.Redirect(ctx.RedirectUri);
                        }

                        return Task.FromResult(0);
                    }
                };
            })
            .AddEntityFrameworkStores<WorldContext>();

            services.AddLogging();
            services.AddCaching();

            services.AddEntityFramework()
                .AddSqlServer()
                .AddDbContext<WorldContext>();

            services.AddScoped<ICacheManager, CacheManager>();
            services.AddTransient<WorldContextSeedData>();
            services.AddScoped<IWorldRepository, WorldRepository>();
#if DEBUG
            services.AddScoped<IMailService, DebugMailService>();
#else
            services.AddScoped<IMailService, RealMailService>();
#endif
            services.AddScoped<ICoordService, BingCoordService>();

            services.ConfigureRouting(routeOptions =>
            {
                routeOptions.AppendTrailingSlash = true;
                routeOptions.LowercaseUrls = true;
            });
        }

        //This method is invoked when ASPNET_ENV is 'Development' or is not defined
        //The allowed values are Development,Staging and Production
        public async void ConfigureDevelopment(IApplicationBuilder app, ILoggerFactory loggerFactory, WorldContextSeedData seeder, IHostingEnvironment env)
        {
            loggerFactory.AddConsole(minLevel: LogLevel.Verbose);

            // StatusCode pages to gracefully handle status codes 400-599.
            //app.UseStatusCodePagesWithRedirects("~/Home/StatusCodePage");

            // Display custom error page in production when error occurs
            // During development use the ErrorPage middleware to display error information in the browser
            app.UseDeveloperExceptionPage();

            app.UseDatabaseErrorPage();

            // Add the runtime information page that can be used by developers
            // to see what packages are used by the application
            // default path is: /runtimeinfo
            app.UseRuntimeInfoPage();

            await Configure(app, seeder, env);
        }

        //This method is invoked when ASPNET_ENV is 'Staging'
        //The allowed values are Development,Staging and Production
        public async void ConfigureStaging(IApplicationBuilder app, ILoggerFactory loggerFactory, WorldContextSeedData seeder, IHostingEnvironment env)
        {
            loggerFactory.AddConsole(minLevel: LogLevel.Warning);

            // StatusCode pages to gracefully handle status codes 400-599.
            app.UseStatusCodePagesWithRedirects("~/Home/StatusCodePage");

            app.UseExceptionHandler("/Home/Error");

            await Configure(app, seeder, env);
        }

        //This method is invoked when ASPNET_ENV is 'Production'
        //The allowed values are Development,Staging and Production
        public async void ConfigureProduction(IApplicationBuilder app, ILoggerFactory loggerFactory, WorldContextSeedData seeder, IHostingEnvironment env)
        {
            loggerFactory.AddConsole(minLevel: LogLevel.Warning);

            // StatusCode pages to gracefully handle status codes 400-599.
            app.UseStatusCodePagesWithRedirects("~/Home/StatusCodePage");

            app.UseExceptionHandler("/Home/Error");

            await Configure(app, seeder, env);
        }

        public async Task Configure(IApplicationBuilder app, WorldContextSeedData seeder, IHostingEnvironment env)
        {
            app.UseStaticFiles();

            app.UseIdentity();

            app.UseMvc(config =>
            {
                config.MapRoute(
                    name: "Default",
                    template: "{controller}/{action}/{id?}",
                    defaults: new { controller = "App", action = "Index" }
                );
            });

            Mapper.Initialize(config =>
            {
                config.CreateMap<Trip, TripViewModel>().ReverseMap();
                config.CreateMap<Stop, StopViewModel>().ReverseMap();
            });

            await seeder.EnsureSeedDataAsync();
        }

        // Entry point for the application.
        public static void Main(string[] args) => WebApplication.Run<Startup>(args);
    }
}