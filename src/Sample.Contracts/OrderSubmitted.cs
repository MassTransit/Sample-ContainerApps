namespace Sample.Contracts;

public record OrderSubmitted(Guid OrderId) :
    OrderMessage(OrderId);