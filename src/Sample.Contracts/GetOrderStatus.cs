namespace Sample.Contracts;

public record GetOrderStatus(Guid OrderId) : 
    OrderMessage(OrderId);