using System.Text;
using System.Text.Json;
using EscalationService.Appliacation.Common.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Models;
using Models.Options;
using RabbitMQ.Client;

namespace EscalationService.Infrastructure.MessageBus;

public class UserIdsPublisher : IMessageBusPublisher, IAsyncDisposable
{
    private readonly RabbitMqOptions _rabbitMqOptions;
    private readonly IConnection _connection;
    private readonly IChannel _channel;
    private const string QueueName = "user_ids_queue";

    
    public UserIdsPublisher(IOptions<RabbitMqOptions> options)
    {
        _rabbitMqOptions = options.Value;
        var factory = new ConnectionFactory
        {
            HostName = _rabbitMqOptions.HostName,
            UserName = _rabbitMqOptions.UserName,
            Password = _rabbitMqOptions.Password
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
            autoDelete: false,
            arguments: null); 
    }
    
    public async Task PublishUserIds(List<int> userIds)
    {
        var message = JsonSerializer.Serialize(userIds);
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
        await _channel.DisposeAsync();
        _connection.Dispose();
    }

}