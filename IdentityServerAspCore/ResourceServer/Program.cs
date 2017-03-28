using System;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Builder;

namespace ResourceServer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.Title = "Resource Server";

            var host = new WebHostBuilder()
                .UseKestrel()
                .UseUrls("http://0.0.0.0:5001")
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseIISIntegration()
                .UseStartup<Startup>()
                .Build();

            host.Run();
        }
    }
}
