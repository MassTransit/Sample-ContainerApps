namespace Sample.Contracts;

public record OrderStatus(Guid OrderId, string Status, DateTime? Submitted, DateTime? Fulfilled) :
    OrderMessage(OrderId);