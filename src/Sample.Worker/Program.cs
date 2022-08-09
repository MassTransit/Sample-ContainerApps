using MassTransit;
using Sample;
using Sample.Contracts;

var host = Host.CreateDefaultBuilder(args)
    .UseSerilogConfiguration()
    .UseMassTransitConfiguration(x =>
    {
        x.AddHandler(async (SubmitOrder order) => new OrderSubmissionAccepted(order.OrderId));
    })
    .Build();

await host.RunAsync();