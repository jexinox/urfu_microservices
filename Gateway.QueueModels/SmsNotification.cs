namespace Gateway.QueueModels;

public record SmsNotification(Guid Id, string Number, string Message);