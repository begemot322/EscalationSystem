using Models.Options;
using SchedulerService.API.Contracts;
using SchedulerService.API.MessageBus;
using SchedulerService.API.Services;

namespace SchedulerService.API;

public static class DependencyInjection
{
    public static IServiceCollection AddServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddControllers();
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();   
        
        // Configure
        services.Configure<RabbitMqOptions>(configuration.GetSection("RabbitMq"));
        

        services.AddHttpContextAccessor();
        services.AddHttpClient<IEscalationServiceClient, EscalationServiceClient>(client => 
        {
            client.BaseAddress = new Uri(configuration["EscalationService:BaseUrl"]!);
        });
        
        services.AddSingleton<IEscalationPublisher, EscalationPublisher>();

        services.AddHostedService<EscalationCheckerService>();

        return services;
    }
}