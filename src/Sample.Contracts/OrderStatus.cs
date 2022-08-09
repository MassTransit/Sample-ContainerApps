namespace Sample.Contracts;

public record OrderStatus(Guid OrderId, string Status) :
    OrderMessage(OrderId);