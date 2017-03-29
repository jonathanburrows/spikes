using System;
using System.IO;
using Microsoft.AspNetCore.Hosting;

namespace AuthorizationServer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.Title = "Authorization Server";

            var host = new WebHostBuilder()
                .UseKestrel()
                .UseUrls("https://0.0.0.0:44300")
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseIISIntegration()
                .UseStartup<Startup>()
                .Build();

            host.Run();
        }
    }
}
