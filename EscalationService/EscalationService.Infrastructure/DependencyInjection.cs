using EscalationService.Appliacation.Common.Interfaces.Repositories;
using EscalationService.Infrastructure.Data;
using EscalationService.Infrastructure.Data.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EscalationService.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddData(configuration); 

        return services;
    }
    
    private static IServiceCollection AddData(this IServiceCollection services,
        IConfiguration configuration)
    {
        //База данных
        var connString = configuration.GetConnectionString("Database");
        services.AddDbContext<ApplicationDbContext>(options => 
            options.UseNpgsql(connString));
        
        //Репозитории
        services.AddScoped<IEscalationRepository, EscalationRepository>();
        services.AddScoped<ICriteriaRepository, CriteriaRepository>();
        
        return services;
    }
}