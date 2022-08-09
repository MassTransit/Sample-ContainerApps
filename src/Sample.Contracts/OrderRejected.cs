namespace Sample.Contracts;

public record OrderRejected(Guid OrderId) :
    OrderMessage(OrderId);