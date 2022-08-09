namespace Sample.Worker.StateMachines;

using MassTransit;


public class OrderStateDefinition :
    SagaDefinition<OrderState>
{
    protected override void ConfigureSaga(IReceiveEndpointConfigurator endpointConfigurator, ISagaConfigurator<OrderState> sagaConfigurator)
    {
        if (endpointConfigurator is IServiceBusEndpointConfigurator sbc)
        {
            sbc.RequiresSession = true;
            sbc.LockDuration = TimeSpan.FromMinutes(2);
            sbc.MaxAutoRenewDuration = TimeSpan.FromMinutes(5);
            sbc.SessionIdleTimeout = TimeSpan.FromSeconds(5);
        }

        endpointConfigurator.UseMessageRetry(r => r.Intervals(100, 200, 500, 1000));
        endpointConfigurator.UseInMemoryOutbox();
    }
}