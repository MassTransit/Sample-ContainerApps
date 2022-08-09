using MassTransit;
using Sample;
using Sample.Worker.Consumers;
using Sample.Worker.StateMachines;

var host = Host.CreateDefaultBuilder(args)
    .UseSerilogConfiguration()
    .UseMassTransitConfiguration(x =>
    {
        x.AddConsumer<SubmitOrderConsumer, SubmitOrderConsumerDefinition>();
        x.AddConsumer<ValidationConsumer>();

        x.AddSagaStateMachine<OrderStateMachine, OrderState, OrderStateDefinition>()
            .MessageSessionRepository();
    })
    .Build();

await host.RunAsync();