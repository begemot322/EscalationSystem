using System.Text.Json;
using Microsoft.Extensions.Options;
using Models.DTOs;
using Models.Options;
using NotificationService.API.Contracts;
using NotificationService.API.Models;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace NotificationService.API.MessageBus;

public class OverdueEscalationConsumer : BackgroundService
{
    private readonly IConnection _connection;
    private readonly IChannel _channel;
    private readonly IEmailService _emailService;
    private readonly RabbitMqOptions _rabbitMqOptions;
    private readonly IUserServiceClient _userClient;
    private const string QueueName = "overdue_escalations_queue";

    public OverdueEscalationConsumer(
        IOptions<RabbitMqOptions> options,
        IEmailService emailService,
        IUserServiceClient userClient)
    {
        _userClient = userClient;
        _rabbitMqOptions = options.Value;
        _emailService = emailService;

        var factory = new ConnectionFactory
        {
            HostName = _rabbitMqOptions.HostName,
            UserName = _rabbitMqOptions.UserName,
            Password = _rabbitMqOptions.Password
        };

        _connection = factory.CreateConnectionAsync().GetAwaiter().GetResult();
        _channel = _connection.CreateChannelAsync().GetAwaiter().GetResult();

        _channel.QueueDeclareAsync(
            queue: QueueName,
            durable: true,
            exclusive: false,
            autoDelete: false);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        stoppingToken.ThrowIfCancellationRequested();
        
        var consumer = new AsyncEventingBasicConsumer(_channel);

        consumer.ReceivedAsync += async (_, ea) =>
        {
            var body = ea.Body.ToArray();
            var dto = JsonSerializer.Deserialize<EscalationReminderDto>(body);

            var userIds = new List<int> { dto.AuthorId };
            userIds.AddRange(dto.ResponsibleUserIds);
            var users = await _userClient.GetUsersByIdsAsync(userIds);

            foreach (var user in users)
            {
                var isAuthor = user.Id == dto.AuthorId;
                var emailRequest = new SendEmailRequest(
                    user.Email,
                    isAuthor ? "СРОЧНО: Просроченная эскалация" : "Напоминание об эскалации",
                    BuildEmailHelper(user, dto, isAuthor));

                await _emailService.SendEmailAsync(emailRequest);
            }

            await _channel.BasicAckAsync(ea.DeliveryTag, false);
        };
        await _channel.BasicConsumeAsync(QueueName, autoAck: false, consumer, cancellationToken: stoppingToken);
    }
    private string BuildEmailHelper(UserDto user, EscalationReminderDto dto, bool isAuthor)
    {
        return isAuthor
            ? $"Уважаемый(ая) {user.FirstName} {user.LastName}, вы автор просроченной эскалации {dto.Name}" +
              $" (Прошло более 30 дней. Требуются срочные действия!"
            : $"Уважаемый(ая) {user.FirstName} {user.LastName}, эскалация {dto.Name} просрочена (прошло более 30 дней." +
              $" Пожалуйста, поговорите с автором эскалации или вашим Team lead.";
    }
}