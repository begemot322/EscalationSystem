using EscalationService.API;
using EscalationService.Appliacation;
using EscalationService.Appliacation.Common.Interfaces;
using EscalationService.Appliacation.Services.Interfaces;
using EscalationService.Infrastructure;
using EscalationService.Infrastructure.MessageBus;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddInfrastructureServices(builder.Configuration)
    .AddApplicationServices()
    .AddWebServices();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();   
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.MapGet("/internal/escalation/overdue",
    async (IEscalationService escalationService) =>
    {
        var result = await escalationService.GetOverdueEscalationsAsync();
        return Results.Ok(result.Data);
    });

using (var scope = app.Services.CreateScope())
{
    var publisher = scope.ServiceProvider.GetRequiredService<IMessageBusPublisher>() as UserIdsPublisher;
    if (publisher != null)
        await publisher.InitializeAsync();
}

app.Run();