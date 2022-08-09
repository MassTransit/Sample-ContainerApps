namespace Sample.Worker.StateMachines;

using MassTransit;


public class OrderState :
    SagaStateMachineInstance
{
    public string? CurrentState { get; set; }

    public DateTime? Submitted { get; set; }
    public DateTime? Fulfilled { get; set; }

    public TimeSpan FulfillmentDelay { get; set; }

    public Guid? ScheduleTokenId { get; set; }
    public Guid CorrelationId { get; set; }
}