using NotificationService.API.Models;

namespace NotificationService.API.Contracts;

public interface IEmailService
{
    Task SendEmailAsync(SendEmailRequest sendEmailRequest);

}