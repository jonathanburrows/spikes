using AccessCodeClient.Http;
using AccessCodeClient.Options;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;

namespace AccessCodeClient
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

            if (env.IsDevelopment())
            {
                builder.AddApplicationInsightsSettings(developerMode: true);
            }
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddApplicationInsightsTelemetry(Configuration)
                .AddScoped<IHttpContextAccessor, HttpContextAccessor>()
                .AddScoped<OidcHttpClientFactory>()
                .Configure<ClientOptions>(options => options.ResourceServerUrl = "http://localhost:5001")
                .AddMvc();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

             app.UseApplicationInsightsRequestTelemetry() 
                .UseDeveloperExceptionPage()
                .UseBrowserLink()
                .UseApplicationInsightsExceptionTelemetry()
                .UseCookieAuthentication(new CookieAuthenticationOptions { AuthenticationScheme = "Cookies" })
                .UseOpenIdConnectAuthentication(new OpenIdConnectOptions
                {
                    AuthenticationScheme = "oidc",
                    SignInScheme = "Cookies",

                    Authority = "http://localhost:5000",
                    RequireHttpsMetadata = false,

                    ClientId = "access-code",
                    ClientSecret = "secret",

                    ResponseType = "code id_token",
                    Scope = { "resource-server", "offline_access", "openid", "profile" },
                    PostLogoutRedirectUri = "http://localhost:5002",

                    GetClaimsFromUserInfoEndpoint = true,
                    SaveTokens = true
                })
                .UseStaticFiles()
                .UseMvcWithDefaultRoute();
        }
    }
}
