using System.Security.Claims;
using System.Text;
using EscalationService.Appliacation.Common.Interfaces;
using EscalationService.Appliacation.Common.Interfaces.Repositories;
using EscalationService.Infrastructure.Data;
using EscalationService.Infrastructure.Data.Repositories;
using EscalationService.Infrastructure.MessageBus;
using EscalationService.Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Models;
using Models.Options;

namespace EscalationService.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddHttpContextAccessor();
    
        services.AddHttpClient<IUserServiceClient, UserServiceClient>(client => 
        {
            client.BaseAddress = new Uri(configuration["UserService:BaseUrl"]!);
        });
        
        // Rabbit
        services.Configure<RabbitMqOptions>(configuration.GetSection("RabbitMq"));
        services.AddSingleton<IMessageBusPublisher, UserIdsPublisher>();
        services.AddHostedService<EscalationReportConsumer>();

        services.AddRedis(configuration);

        services.AddData(configuration); 
        
        services.AddAuth(configuration);

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
        services.AddScoped<ICommentRepository, CommentRepository>();
        services.AddScoped<IEscalationUserRepository, EscalationUserRepository>();

        services.AddScoped<IUnitOfWork, UnitOfWork>();
        
        return services;
    }

    private static IServiceCollection AddRedis(this IServiceCollection services,
        IConfiguration configuration)
    {
        var redisConnection = configuration["Redis"];
        
        services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = redisConnection;
            options.InstanceName = "EscalationService";
        });
        
        services.AddScoped<IRedisCacheService, RedisCacheService>();
        
        return services;
    }
    
    private static IServiceCollection AddAuth(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<JwtOptions>(configuration.GetSection("JwtOptions"));
        var jwtOptions = configuration.GetSection(nameof(JwtOptions)).Get<JwtOptions>();

        services
            .AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = jwtOptions.Issuer,
                    ValidateAudience = true,
                    ValidAudience = jwtOptions.Audience,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.SecretKey)),
                    RoleClaimType = ClaimTypes.Role
                };

                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        context.Token = context.Request.Cookies["SecurityCookies"];
                        return Task.CompletedTask;
                    }
                };
            });

        services.AddScoped<IUserContext, UserContext>();
        return services;
    }
}