namespace Sample.Worker.Consumers;

using Contracts;
using MassTransit;


public class SubmitOrderConsumer :
    IConsumer<SubmitOrder>
{
    readonly ILogger<SubmitOrderConsumer> _logger;

    public SubmitOrderConsumer(ILogger<SubmitOrderConsumer> logger)
    {
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<SubmitOrder> context)
    {
        var orderId = context.Message.OrderId;

        _logger.LogInformation("Consuming Submit Order: {OrderId}", orderId);

        await context.Publish(new OrderSubmitted(orderId));

        if (context.IsResponseAccepted<OrderSubmissionAccepted>())
            await context.RespondAsync(new OrderSubmissionAccepted(orderId));
    }
}