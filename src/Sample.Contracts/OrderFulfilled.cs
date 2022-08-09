namespace Sample.Contracts;

public record OrderFulfilled(Guid OrderId) :
    OrderMessage(OrderId);