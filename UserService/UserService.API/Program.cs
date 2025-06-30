using Microsoft.AspNetCore.CookiePolicy;
using UserService.API;
using UserService.Application;
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

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();