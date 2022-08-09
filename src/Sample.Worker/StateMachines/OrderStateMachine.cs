namespace Sample.Worker.StateMachines;

using Contracts;
using MassTransit;

#pragma warning disable CS8618
public class OrderStateMachine :
    MassTransitStateMachine<OrderState>
{
    public OrderStateMachine(ILogger<OrderStateMachine> logger)
    {
        InstanceState(x => x.CurrentState);

        Event(() => OrderSubmitted, x => x.CorrelateById(context => context.Message.OrderId));
        Event(() => OrderStatusRequested, x =>
        {
            x.CorrelateById(context => context.Message.OrderId);
            x.OnMissingInstance(m => m.ExecuteAsync(context => context.RespondAsync(new OrderNotFound(context.Message.OrderId))));
        });

        Schedule(() => OrderFulfilled, x => x.ScheduleTokenId, x =>
        {
            x.DelayProvider = context => context.Saga.FulfillmentDelay;

            x.Received = config =>
            {
                config.ConfigureConsumeTopology = false;
                config.CorrelateById(context => context.Message.OrderId);
            };
        });

        Request(() => ValidateRequest, x =>
        {
            x.Timeout = TimeSpan.Zero;
        });

        Initially(
            When(OrderSubmitted)
                .Then(context =>
                {
                    context.Saga.Submitted = context.SentTime ?? DateTime.UtcNow;
                    context.Saga.FulfillmentDelay = context.Message.FulfillmentDelay ?? TimeSpan.FromMinutes(Random.Shared.Next(1, 30));
                })
                .TransitionTo(Submitted)
                .Request(ValidateRequest, x => new ValidateOrder(x.Saga.CorrelationId))
        );

        During(Submitted,
            When(ValidateRequest!.Completed)
                .Publish(x => new OrderAccepted(x.Saga.CorrelationId))
                .Schedule(OrderFulfilled, context => new OrderFulfilled(context.Saga.CorrelationId))
                .Then(context =>
                {
                    logger.LogInformation("Scheduling fulfillment: {OrderId} {Delay}", context.Saga.CorrelationId, context.Saga.FulfillmentDelay);
                })
                .TransitionTo(Accepted),
            When(ValidateRequest.Faulted)
                .Publish(x => new OrderRejected(x.Saga.CorrelationId))
                .TransitionTo(Rejected)
        );

        During(Accepted,
            When(OrderFulfilled!.Received)
                .Then(context =>
                {
                    context.Saga.Fulfilled = context.SentTime ?? DateTime.UtcNow;

                    logger.LogInformation("Order fulfilled: {OrderId}", context.Saga.CorrelationId);
                })
                .TransitionTo(Fulfilled));

        DuringAny(
            When(OrderStatusRequested)
                .RespondAsync(async x => new OrderStatus(x.Saga.CorrelationId, (await x.StateMachine.GetState(x)).Name, x.Saga.Submitted, x.Saga.Fulfilled))
        );
    }

    //
    // ReSharper disable UnassignedGetOnlyAutoProperty
    // ReSharper disable MemberCanBePrivate.Global
    public State Submitted { get; }
    public State Accepted { get; }
    public State Fulfilled { get; }
    public State Rejected { get; }
    public Event<OrderSubmitted> OrderSubmitted { get; }
    public Event<GetOrderStatus> OrderStatusRequested { get; }
    public Request<OrderState, ValidateOrder, OrderValidated> ValidateRequest { get; }
    public Schedule<OrderState, OrderFulfilled> OrderFulfilled { get; }
}