using AuthorizationServer.Middleware;
using AuthorizationServer.Models;
using IdentityServer4;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
    
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

            var requiredSecrets = new[] {
                "Authorization:Facebook:ClientId",
                "Authorization:Facebook:ClientSecret",
                "Authorization:Google:ClientId",
                "Authorization:Google:ClientSecret",
                "Authorization:Microsoft:ClientId",
                "Authorization:Microsoft:ClientSecret"
            };
            foreach (var requiredSecret in requiredSecrets)
            {
                if (Configuration[requiredSecret] == null)
                {
                    throw new ArgumentException($"The variable {requiredSecret} has not been set");
                }
            }
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
                .AddCors()
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
                .UseCors(options => options.AllowAnyOrigin())
                .UseIdentityServer()
                .UseFacebookAuthentication(new FacebookOptions
                {
                    AuthenticationScheme = "Facebook",
                    DisplayName = "Facebook",
                    SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme,

                    AppId = Configuration["Authorization:Facebook:ClientId"],
                    AppSecret = Configuration["Authorization:Facebook:ClientSecret"]
                })
                .UseGoogleAuthentication(new GoogleOptions
                {
                    AuthenticationScheme = "Google",
                    DisplayName = "Google",
                    SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme,
                    ClientId = Configuration["Authorization:Google:ClientId"],
                    ClientSecret = Configuration["Authorization:Google:ClientSecret"]
                })
                .UseMicrosoftAccountAuthentication(new MicrosoftAccountOptions
                {
                    AuthenticationScheme = "Microsoft",
                    DisplayName = "Microsoft",
                    SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme,
                    ClientId = Configuration["Authorization:Microsoft:ClientId"],
                    ClientSecret = Configuration["Authorization:Microsoft:ClientSecret"]
                })
                .UseStaticFiles()
                .UseMvc(routes => routes.MapRoute("default", "{controller=Home}/{action=Index}/{id?}"))
                .UseMiddleware<SecurityHeaderMiddleware>();
        }
    }
}

