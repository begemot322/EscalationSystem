using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Options;
using Models.DTOs;
using Models.Options;
using RabbitMQ.Client;
using SchedulerService.API.Contracts;

namespace SchedulerService.API.MessageBus;

public class EscalationPublisher : IEscalationPublisher, IAsyncDisposable
{
    private readonly IConnection _connection;
    private readonly IChannel _channel;
    private readonly RabbitMqOptions _options;
    private const string QueueName = "overdue_escalations_queue";
    
    public EscalationPublisher(
        IOptions<RabbitMqOptions> options,
        IEscalationServiceClient escalationClient)
    {
        _options = options.Value;
        
        var factory = new ConnectionFactory
        {
            HostName = _options.HostName,
            UserName = _options.UserName,
            Password = _options.Password
        };
        
        _connection =  factory.CreateConnectionAsync().GetAwaiter().GetResult();
        _channel = _connection.CreateChannelAsync().GetAwaiter().GetResult();
    }
    
    public async Task InitializeAsync()
    {
        await _channel.QueueDeclareAsync(
            queue: QueueName,
            durable: true,
            exclusive: false,
            autoDelete: false); 
    }
    
    public async Task PublishOverdueEscalationAsync(EscalationReminderDto escalation)
    {
        var message = JsonSerializer.Serialize(escalation);
        var body = Encoding.UTF8.GetBytes(message);
        
        await _channel.BasicPublishAsync(
            exchange: "",
            routingKey: QueueName,
            mandatory:true,
            basicProperties:new BasicProperties {Persistent = true},
            body: body);
    }
    

    public async ValueTask DisposeAsync()
    {
        await _connection.DisposeAsync();
        await _channel.DisposeAsync();
    }
}