using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using UserService.Application.DTOs;
using UserService.Application.Services.Implementation;
using UserService.Application.Services.Interfaces;

namespace UserService.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IAuthService,AuthService>();
        
        
        // Валидация
        services.AddValidatorsFromAssemblyContaining<LoginUserDto>();

        return services;
    }

}