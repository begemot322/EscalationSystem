using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Options;
using Models.DTOs;
using Models.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using ReportingService.API.Contracts;

namespace ReportingService.API.MessageBus;

public class ReportPublisher : IMessageBusPublisher ,IAsyncDisposable
{
    private readonly RabbitMqOptions _rabbitMqOptions;
    private readonly IConnection _connection;
    private readonly IChannel _channel;
    
    private const string QueueName = "reporting_queue";
    private const string Exchange = "reporting_exchange";
    
    public ReportPublisher(IOptions<RabbitMqOptions> options)
    {
        _rabbitMqOptions = options.Value;
        var factory = new ConnectionFactory
        {
            HostName = _rabbitMqOptions.HostName,
            UserName = _rabbitMqOptions.UserName,
            Password = _rabbitMqOptions.Password
        };
        
        _connection = Task.Run(() => factory.CreateConnectionAsync()).Result;
        _channel = Task.Run(() => _connection.CreateChannelAsync()).Result;
    }
    
    public async Task InitializeAsync()
    {
       await _channel.ExchangeDeclareAsync(
            exchange: Exchange,
            type: ExchangeType.Direct,
            durable: true,
            autoDelete: false);
        
        await _channel.QueueDeclareAsync(
            queue: QueueName,
            durable: true,
            exclusive: false,
            autoDelete: false);
        
        await _channel.QueueBindAsync(
            queue: QueueName,
            exchange: Exchange,
            routingKey: "escalation.get_all");
    }

    public async Task<List<EscalationDtoMessage>> GetEscalationsAsync(ReportRequest request)
    {
        var replyQueue = await _channel.QueueDeclareAsync
            ("", durable: false, exclusive: true, autoDelete: true);
        
        var props = new BasicProperties
        {
            ReplyTo = replyQueue,
            CorrelationId =  Guid.NewGuid().ToString(),
        };
        
        var tcs = new TaskCompletionSource<List<EscalationDtoMessage>>();
        
        var consumer = new AsyncEventingBasicConsumer(_channel);
        consumer.ReceivedAsync += (model, ea) =>
        {
            if (ea.BasicProperties.CorrelationId == props.CorrelationId)
            {
                var response = JsonSerializer.Deserialize<List<EscalationDtoMessage>>(ea.Body.Span);
                tcs.SetResult(response);
            }
            return Task.CompletedTask; 
        };
        await _channel.BasicConsumeAsync(replyQueue.QueueName, autoAck: true, consumer);
        
        var message = JsonSerializer.Serialize(request);
        await _channel.BasicPublishAsync(
            exchange: Exchange,
            routingKey: "escalation.get_all",
            mandatory:true,
            basicProperties:props,
            body: Encoding.UTF8.GetBytes(message));
        
        return await tcs.Task;
    }

    public async ValueTask DisposeAsync()
    {
        await _channel.DisposeAsync();
        _connection.Dispose();
    }
}