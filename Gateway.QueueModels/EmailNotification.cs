namespace Gateway.QueueModels;

public record EmailNotification(Guid Id, string Address, string Subject, string Body);
