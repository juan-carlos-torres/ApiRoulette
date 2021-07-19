using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace RouletteApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
            .UseSerilog(
                    (hostingContext, loggerConfiguration) =>
                    {
                        var outputTemplate = "[{Timestamp:hh:mm:ss tt}] {Message:lj}{NewLine}{NewLine}{Exception}{NewLine}{NewLine}{NewLine}{NewLine}";

                        loggerConfiguration
                                .MinimumLevel.Error()
                                .WriteTo.RollingFile("Logs/log.log", outputTemplate: outputTemplate);
                    })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
