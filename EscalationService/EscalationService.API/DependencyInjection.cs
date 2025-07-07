using System.Net.Http.Headers;
using EscalationService.Appliacation.Common.Interfaces;
using EscalationService.Infrastructure;

namespace EscalationService.API;

public static class DependencyInjection
{
    public static IServiceCollection AddWebServices(this IServiceCollection services)
    {
        services.AddControllers();
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();   
        
        return services;
    }
}