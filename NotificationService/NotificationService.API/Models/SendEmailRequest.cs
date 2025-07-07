namespace NotificationService.API.Models;

public record SendEmailRequest(string Recipient, string Subject, string Body);