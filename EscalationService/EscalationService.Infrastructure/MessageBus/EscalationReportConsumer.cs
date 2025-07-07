using System.Text;
using System.Text.Json;
using EscalationService.Appliacation.Common.Interfaces.Repositories;
using EscalationService.Appliacation.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Models.DTOs;
using Models.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace EscalationService.Infrastructure.MessageBus;

public class EscalationReportConsumer: BackgroundService
{
    private readonly IConnection _connection;
    private readonly IChannel _channel;
    private readonly RabbitMqOptions _options;
    private readonly IServiceProvider _serviceProvider;
    
    private const string QueueName = "reporting_queue";
    private const string Exchange = "reporting_exchange";

    public EscalationReportConsumer(
        IOptions<RabbitMqOptions> options,
        IServiceProvider serviceProvider)
    {
        _options = options.Value;
        _serviceProvider = serviceProvider;
        
        var factory = new ConnectionFactory
        {
            HostName = _options.HostName,
            UserName = _options.UserName,
            Password = _options.Password
        };
        
        _connection =  factory.CreateConnectionAsync().GetAwaiter().GetResult();
        _channel = _connection.CreateChannelAsync().GetAwaiter().GetResult();
        
         _channel.ExchangeDeclareAsync(Exchange,ExchangeType.Direct, true, false);
        _channel.QueueDeclareAsync(QueueName, durable: true, exclusive: false, autoDelete: false);
        _channel.QueueBindAsync(QueueName, Exchange, routingKey: "escalation.get_all");
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        stoppingToken.ThrowIfCancellationRequested();

        var consumer = new AsyncEventingBasicConsumer(_channel);

        consumer.ReceivedAsync += async (_, ea) =>
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var service = scope.ServiceProvider.GetRequiredService<IEscalationService>();
                
                var body = ea.Body.ToArray();
                var request = JsonSerializer.Deserialize<ReportRequest>(body);

                var result  = await service.GetFilteredEscalationsAsync(
                    request?.FromDate,
                    request?.ToDate,
                    request?.Status);

                var escalations = result.Data;

                var replyProps = new BasicProperties
                {
                    CorrelationId = ea.BasicProperties.CorrelationId,
                };

                await _channel.BasicAckAsync(ea.DeliveryTag, multiple: false);
                
                await _channel.BasicPublishAsync(
                    exchange: "",
                    routingKey: ea.BasicProperties.ReplyTo,
                    mandatory: true,
                    basicProperties: replyProps,
                    body: JsonSerializer.SerializeToUtf8Bytes(escalations),
                    cancellationToken: stoppingToken
                );
            }
        };
        await _channel.BasicConsumeAsync(QueueName, autoAck: false, consumer, cancellationToken: stoppingToken);
    }
}