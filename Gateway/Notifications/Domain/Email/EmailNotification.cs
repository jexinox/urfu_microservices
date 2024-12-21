using System.Net.Mail;

namespace Gateway.Notifications.Domain.Email;

public record EmailNotification(MailAddress Address, string Subject, string Body);
