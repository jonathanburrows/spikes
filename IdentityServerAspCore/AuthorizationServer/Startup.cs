using AuthorizationServer.Middleware;
using AuthorizationServer.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace AuthorizationServer
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();

            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            var windowsProviders = new[]
            {
                new ExternalProvider { AuthenticationScheme = "NTLM", DisplayName = "Windows" }
            };

            services.AddAuthorization(options => options.WindowsProviders = windowsProviders);

            services.AddMvc(options => options.Filters.Add(new RequireHttpsAttribute()));

            var testStore = new Stores.TestStore();
            services
                .AddSingleton(testStore)
                .AddIdentityServer()
                .AddTemporarySigningCredential()
                .AddInMemoryPersistedGrants()
                .AddInMemoryApiResources(testStore.GetApiResources())
                .AddInMemoryIdentityResources(testStore.GetIdentityResources())
                .AddInMemoryClients(testStore.GetClients())
                .AddTestUsers(testStore.GetUsers());
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            app.UseDeveloperExceptionPage()
                //.UseIdentity()
                .UseIdentityServer()
                .UseStaticFiles()
                .UseMvc(routes => routes.MapRoute("default", "{controller=Home}/{action=Index}/{id?}"))
                .UseMiddleware<SecurityHeaderMiddleware>();
        }
    }
}
