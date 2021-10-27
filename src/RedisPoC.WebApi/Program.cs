namespace RedisPoC.WebApi
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Hosting;
    using Serilog;

    [ExcludeFromCodeCoverage]
    public static class Program
    {
        public static async Task Main(string[] args)
        {
            var seqAddress = Environment.GetEnvironmentVariable("SEQ_ADDRESS") ?? "https://localhost:44375";

            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .AddEnvironmentVariables()
                .Build();

            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(configuration)
                .WriteTo.Seq(seqAddress)
                .CreateLogger();

            try
            {
                Log.Information("Starting host...");
                await CreateHostBuilder(args).Build().RunAsync();
            }
            catch (Exception exception)
            {
                Log.Fatal(exception, "Host terminated unexpectedly.");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseSerilog()
                .ConfigureAppConfiguration((_, config) => { config.AddEnvironmentVariables(prefix: "CONFIG_"); })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                    webBuilder.UseKestrel(options =>
                    {
                        options.Limits.MaxRequestBodySize = int.MaxValue;
                    });
                });
    }
}
