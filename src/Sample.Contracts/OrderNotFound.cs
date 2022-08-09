namespace Sample.Contracts;

public record OrderNotFound(Guid OrderId) :
    OrderMessage(OrderId);