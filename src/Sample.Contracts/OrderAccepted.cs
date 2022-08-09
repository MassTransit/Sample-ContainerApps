namespace Sample.Contracts;

public record OrderAccepted(Guid OrderId) :
    OrderMessage(OrderId);