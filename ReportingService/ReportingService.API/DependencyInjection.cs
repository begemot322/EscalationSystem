using Models.Options;
using ReportingService.API.Contracts;
using ReportingService.API.MessageBus;
using ReportingService.API.Services;

namespace ReportingService.API;


public static class DependencyInjection
{
    public static IServiceCollection AddServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddControllers();
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();   
        
        // Configure
        services.Configure<RabbitMqOptions>(configuration.GetSection("RabbitMq"));
        services.AddSingleton<IMessageBusPublisher, ReportPublisher>();
        
        // servise
        services.AddScoped<ReportService>();

        return services;
    }
}