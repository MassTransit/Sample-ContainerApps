namespace Sample.Worker.Tests;

using Consumers;
using Contracts;
using MassTransit;
using MassTransit.Testing;
using Microsoft.Extensions.DependencyInjection;
using StateMachines;


[TestFixture]
public class When_request_sent_from_state_machine
{
    [Test]
    public async Task Should_use_correlation_id_for_request_id()
    {
        MessageCorrelation.UseCorrelationId<OrderSubmitted>(message => message.OrderId);
        MessageCorrelation.UseCorrelationId<GetOrderStatus>(message => message.OrderId);

        await using var provider = new ServiceCollection()
            .AddMassTransitTestHarness(x =>
            {
                x.AddConsumer<SubmitOrderConsumer>();
                x.AddConsumer<ValidationConsumer>();

                x.AddSagaStateMachine<OrderStateMachine, OrderState>();
            })
            .BuildServiceProvider(true);

        var harness = provider.GetTestHarness();

        await harness.Start();

        IRequestClient<SubmitOrder>? client = harness.GetRequestClient<SubmitOrder>();

        var orderId = Guid.NewGuid();

        try
        {
            await client.GetResponse<OrderSubmissionAccepted>(new SubmitOrder(orderId));

            ISagaStateMachineTestHarness<OrderStateMachine, OrderState>? sagaHarness = harness.GetSagaStateMachineHarness<OrderStateMachine, OrderState>();

            Assert.That(await sagaHarness.Consumed.Any<OrderValidated>(), Is.True);

            Assert.That(await harness.Published.Any<OrderAccepted>(), Is.True);
        }
        finally
        {
            await harness.Stop();
        }
    }
}