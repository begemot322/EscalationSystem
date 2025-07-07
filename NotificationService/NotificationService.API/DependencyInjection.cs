using Models;
using Models.Options;
using NotificationService.API.Contracts;
using NotificationService.API.MessageBus;
using NotificationService.API.Services;

namespace NotificationService.API;

public static class DependencyInjection
{
    public static IServiceCollection AddServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddControllers();
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();   
        
        // Configure
        services.Configure<RabbitMqOptions>(configuration.GetSection("RabbitMq"));
        services.Configure<GmailOptions>(configuration.GetSection("GmailOptions"));
        
        // Rabbit
        services.AddHostedService<UserNotificationConsumer>();
        // Gmail
        services.AddSingleton<IEmailService, GmailService>();

        services.AddHttpContextAccessor();
        services.AddHttpClient<IUserServiceClient, UserServiceClient>(client => 
        {
            client.BaseAddress = new Uri(configuration["UserService:BaseUrl"]!);
        });

        return services;
    }
}