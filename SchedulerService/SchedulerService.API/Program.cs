using SchedulerService.API;
using SchedulerService.API.Contracts;
using SchedulerService.API.MessageBus;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddServices(builder.Configuration);

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

using (var scope = app.Services.CreateScope())
{
    var publisher = scope.ServiceProvider.GetRequiredService<IEscalationPublisher>() as EscalationPublisher;
    if (publisher != null)
        await publisher.InitializeAsync();
}

app.Run();