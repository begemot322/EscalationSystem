using System.Net.Http.Headers;
using System.Text.Json.Serialization;
using EscalationService.Appliacation.Common.Interfaces;
using EscalationService.Infrastructure;

namespace EscalationService.API;

public static class DependencyInjection
{
    public static IServiceCollection AddWebServices(this IServiceCollection services)
    {
        services.AddControllers()
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve;
                options.JsonSerializerOptions.WriteIndented = true;
            });
        
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();   
        
        
        return services;
    }
}