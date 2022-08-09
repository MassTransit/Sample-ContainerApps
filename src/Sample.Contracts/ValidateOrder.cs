namespace Sample.Contracts;

public record ValidateOrder(Guid OrderId) :
    OrderMessage(OrderId);