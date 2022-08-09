namespace Sample;

using Contracts;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;


public static class MassTransitConfigurationExtensions
{
    public static T UseMassTransitConfiguration<T>(this T builder, Action<IBusRegistrationConfigurator>? configureMassTransit = null,
        Action<IBusRegistrationContext, IServiceBusBusFactoryConfigurator>? configureBus = null)
        where T : IHostBuilder
    {
        builder.UseMassTransit((hostContext, configurator) =>
        {
            configurator.SetKebabCaseEndpointNameFormatter();

            configurator.AddServiceBusMessageScheduler();

            configureMassTransit?.Invoke(configurator);

            configurator.UsingAzureServiceBus((context, cfg) =>
            {
                cfg.UseServiceBusMessageScheduler();

                cfg.Host(hostContext.Configuration.GetConnectionString("ServiceBus"));

                cfg.Publish<OrderMessage>(x => x.Exclude = true);
                cfg.Send<OrderMessage>(s => s.UseSessionIdFormatter(c => c.Message.OrderId.ToString("D")));

                configureBus?.Invoke(context, cfg);

                cfg.ConfigureEndpoints(context);
            });
        });

        return builder;
    }
}