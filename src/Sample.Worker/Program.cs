using MassTransit;
using Sample;
using Sample.Contracts;

var host = Host.CreateDefaultBuilder(args)
    .UseSerilogConfiguration()
    .UseMassTransitConfiguration(x =>
    {
        x.AddHandler(async (SubmitOrder order, ILogger<SubmitOrder> logger) =>
        {
            logger.LogInformation("Consuming Submit Order: {OrderId}", order.OrderId);

            return new OrderSubmissionAccepted(order.OrderId);
        });
    })
    .Build();

await host.RunAsync();