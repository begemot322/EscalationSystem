using Microsoft.AspNetCore.CookiePolicy;
using UserService.API;
using UserService.Application;
using UserService.Application.Services.Interfaces;
using UserService.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.
    AddInfrastructureServices(builder.Configuration)
    .AddApplicationServices()
    .AddWebServices();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();   
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCookiePolicy(new CookiePolicyOptions
{
    MinimumSameSitePolicy = SameSiteMode.Strict,
    HttpOnly = HttpOnlyPolicy.Always,
    Secure = CookieSecurePolicy.Always,
});

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

var internalApi = app.MapGroup("/internal");

internalApi.MapPost("/users/check-exists", 
    async (List<int> userIds, IUserService userService) =>
    {
        var result = await userService.CheckUsersExist(userIds);
        return Results.Ok(result);
    });

internalApi.MapPost("/users/by-ids",
    async (List<int> userIds, IUserService userService) =>
    {
        var users = await userService.GetUsersInfo(userIds);
        return Results.Ok(users);
    });

app.Run();