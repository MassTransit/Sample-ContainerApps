namespace Sample;

using Contracts;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;


public static class MassTransitConfigurationExtensions
{
    public static void UseMassTransitConfiguration(this IHostBuilder builder, Action<IBusRegistrationConfigurator>? callback = null)
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

                cfg.Send<OrderAccepted>(s => s.UseSessionIdFormatter(c => c.Message.OrderId.ToString("D")));
                cfg.Send<OrderNotFound>(s => s.UseSessionIdFormatter(c => c.Message.OrderId.ToString("D")));
                cfg.Send<OrderRejected>(s => s.UseSessionIdFormatter(c => c.Message.OrderId.ToString("D")));
                cfg.Send<OrderStatus>(s => s.UseSessionIdFormatter(c => c.Message.OrderId.ToString("D")));
                cfg.Send<OrderValidated>(s => s.UseSessionIdFormatter(c => c.Message.OrderId.ToString("D")));
                cfg.Send<ValidateOrder>(s => s.UseSessionIdFormatter(c => c.Message.OrderId.ToString("D")));

                cfg.ConfigureEndpoints(context);
            });
        });
    }
}