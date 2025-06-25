using EscalationService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EscalationService.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDatabase(configuration); 

        return services;
    }
    
    private static IServiceCollection AddDatabase(this IServiceCollection services,
        IConfiguration configuration)
    {
        var connString = configuration.GetConnectionString("Database");
        services.AddDbContext<ApplicationDbContext>(options => 
            options.UseNpgsql(connString));
        
        return services;
    }
}