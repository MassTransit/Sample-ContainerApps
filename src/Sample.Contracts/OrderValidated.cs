namespace Sample.Contracts;

public record OrderValidated(Guid OrderId) :
    OrderMessage(OrderId);