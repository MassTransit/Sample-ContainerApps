namespace Sample;

using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;


public static class SerilogConfigurationExtensions
{
    public static T UseSerilogConfiguration<T>(this T builder)
        where T : IHostBuilder
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

        return builder;
    }
}