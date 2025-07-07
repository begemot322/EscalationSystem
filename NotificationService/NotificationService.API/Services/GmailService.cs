using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Options;
using Models.Options;
using NotificationService.API.Contracts;
using NotificationService.API.Models;

namespace NotificationService.API.Services;

public class GmailService : IEmailService
{
    private readonly GmailOptions _gmailOptions;
    
    public GmailService(IOptions<GmailOptions> gmailOptions)
    {
        _gmailOptions = gmailOptions.Value;
    }
    public async Task SendEmailAsync(SendEmailRequest sendEmailRequest)
    {
        MailMessage mailMessage = new MailMessage
        {
            From = new MailAddress(_gmailOptions.Email),
            Subject = sendEmailRequest.Subject,
            Body = sendEmailRequest.Body
        };
        
        mailMessage.To.Add(sendEmailRequest.Recipient);

        using var smtpClient = new SmtpClient();
        smtpClient.Host = _gmailOptions.Host;
        smtpClient.Port = _gmailOptions.Port;
        smtpClient.Credentials = new NetworkCredential(
            _gmailOptions.Email, _gmailOptions.Password);
        smtpClient.EnableSsl = true;

        await smtpClient.SendMailAsync(mailMessage);
    }
}