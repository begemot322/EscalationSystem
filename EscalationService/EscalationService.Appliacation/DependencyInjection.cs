using EscalationService.Appliacation.DTOs;
using EscalationService.Appliacation.Services.Interfaces;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace EscalationService.Appliacation;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services
            .AddServices()
            .AddValidation();

        return services;
    }
    
    private static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddScoped<IEscalationService, Services.Implementation.EscalationService>();
        
        return services;
    }

    private static IServiceCollection AddValidation(this IServiceCollection services)
    {
        services.AddValidatorsFromAssemblyContaining<EscalationDto>();
        
        return services;
    }
}