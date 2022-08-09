namespace Sample.Contracts;

public record SubmitOrder(Guid OrderId, TimeSpan? FulfillmentDelay = default) :
    OrderMessage(OrderId);