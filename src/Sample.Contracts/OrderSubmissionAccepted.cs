namespace Sample.Contracts;

public record OrderSubmissionAccepted(Guid OrderId) :
    OrderMessage(OrderId);
