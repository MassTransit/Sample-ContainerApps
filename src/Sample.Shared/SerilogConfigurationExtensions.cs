namespace Sample;

using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;


public static class SerilogConfigurationExtensions
{
    public static void UseSerilogConfiguration(this IHostBuilder builder)
    {
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Information()
            .MinimumLevel.Override("MassTransit", LogEventLevel.Information)
            .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
            .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
            .Enrich.FromLogContext()
            .WriteTo.Console()
            .CreateLogger();

        builder.UseSerilog();
    }
}