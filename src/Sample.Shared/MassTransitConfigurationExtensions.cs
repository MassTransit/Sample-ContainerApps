namespace Sample;

using Contracts;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;


public static class MassTransitConfigurationExtensions
{
    public static T UseMassTransitConfiguration<T>(this T builder, Action<IBusRegistrationConfigurator>? callback = null)
        where T : IHostBuilder
    {
        builder.UseMassTransit((hostContext, configurator) =>
        {
            configurator.SetKebabCaseEndpointNameFormatter();

            callback?.Invoke(configurator);

            configurator.UsingAzureServiceBus((context, cfg) =>
            {
                cfg.Host(hostContext.Configuration.GetConnectionString("ServiceBus"));

                cfg.Publish<OrderMessage>(x => x.Exclude = true);
                cfg.Send<OrderMessage>(s => s.UseSessionIdFormatter(c => c.Message.OrderId.ToString("D")));

                cfg.ConfigureEndpoints(context);
            });
        });

        return builder;
    }
}