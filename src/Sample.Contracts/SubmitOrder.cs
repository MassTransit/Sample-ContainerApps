namespace Sample.Contracts;

public record SubmitOrder(Guid OrderId) :
    OrderMessage(OrderId);