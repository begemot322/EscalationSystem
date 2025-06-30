using EscalationService.Appliacation.Common.Interfaces.Repositories;
using EscalationService.Appliacation.DTOs;
using EscalationService.Appliacation.Services.Implementation;
using EscalationService.Appliacation.Services.Interfaces;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace EscalationService.Appliacation;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services
            .AddServices();

        return services;
    }
    
    private static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddScoped<IEscalationService, Services.Implementation.EscalationService>();
        services.AddScoped<ICriteriaService, CriteriaService>();
        services.AddScoped<ICommentService, CommentService>();
        
        //Validation
        services.AddValidatorsFromAssemblyContaining<EscalationDto>();
        
        //Automapper
        services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

        return services;
    }
    
}