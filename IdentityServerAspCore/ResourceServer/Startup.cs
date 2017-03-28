using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace ResourceServer
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true);

            if (env.IsEnvironment("Development"))
            {
                builder.AddApplicationInsightsSettings(developerMode: true);
            }

            builder.AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddAspCommon()
                .AddApplicationInsightsTelemetry(Configuration)
                .AddCors()
                .AddMvcCore()
                .AddAuthorization()
                .AddJsonFormatters();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            app.UseApplicationInsightsRequestTelemetry()
                .UseApplicationInsightsExceptionTelemetry()
                .UseIdentityServerAuthentication(new IdentityServerAuthenticationOptions
                {
                    Authority = "http://localhost:5000",
                    RequireHttpsMetadata = false,
                    ApiName = "resource-server",
                    ApiSecret = "secret"
                })
                .UseCors(options => {
                    options.AllowAnyHeader();
                    options.AllowAnyMethod();
                    options.AllowAnyOrigin();
                    options.AllowCredentials();
                })
                .UseMvc();

            PopulateData(app).Wait();
        }

        private async System.Threading.Tasks.Task PopulateData(IApplicationBuilder app) {
            var repository = app.ApplicationServices.GetRequiredService<AspCommon.Repositories.IRepository<Models.Planet>>();

            await repository.CreateAsync(new Models.Planet { Name = "Happy planet" });
            await repository.CreateAsync(new Models.Planet { Name = "sad planet" });
        }
    }
}
