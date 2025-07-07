using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Options;
using Models.Options;
using NotificationService.API.Contracts;
using NotificationService.API.Models;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace NotificationService.API.MessageBus;

public class RabbitMqConsumer : BackgroundService
{
    private readonly IConnection _connection;
    private readonly IChannel _channel;
    private readonly IEmailService _emailService;
    private readonly RabbitMqOptions _rabbitMqOptions;
    private readonly IUserServiceClient _userServiceClient;
    private const string QueueName = "user_ids_queue";

    
    public RabbitMqConsumer(
        IOptions<RabbitMqOptions> options,
        IEmailService emailService,
        IUserServiceClient userServiceClient)
    {
        _rabbitMqOptions = options.Value;
        _emailService = emailService;
        _userServiceClient = userServiceClient;
        
        var factory = new ConnectionFactory
        {
            HostName = _rabbitMqOptions.HostName,
            UserName = _rabbitMqOptions.UserName,
            Password = _rabbitMqOptions.Password
        };
        
        _connection =  factory.CreateConnectionAsync().GetAwaiter().GetResult();
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
            var message = Encoding.UTF8.GetString(body);
            var userIds = JsonSerializer.Deserialize<List<int>>(message);

            if (userIds?.Count > 0)
            {
                var users = await _userServiceClient.GetUsersByIdsAsync(userIds);

                foreach (var user in users)
                {
                    var emailRequest = new SendEmailRequest(
                        user.Email,
                        "Важное уведомление", 
                        $"Здравствуйте {user.FirstName} {user.LastName}, создана эскалация, вы назначены ответственным");
                    
                    await _emailService.SendEmailAsync(emailRequest);
                }
            }
        };
        await _channel.BasicConsumeAsync(QueueName, autoAck: false, consumer, cancellationToken: stoppingToken);

    }
}