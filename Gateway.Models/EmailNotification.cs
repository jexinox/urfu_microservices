using System.Net.Mail;

namespace Gateway.Models;

public record EmailNotification(Guid Id, MailAddress Address, string Subject, string Body);
