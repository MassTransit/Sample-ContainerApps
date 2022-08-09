namespace Sample.Contracts;

public record OrderSubmitted(Guid OrderId, TimeSpan? FulfillmentDelay = default) :
    OrderMessage(OrderId);