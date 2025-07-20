using ReportingService.API;
using ReportingService.API.Contracts;
using ReportingService.API.MessageBus;

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
    var publisher = scope.ServiceProvider.GetRequiredService<IMessageBusPublisher>() as ReportPublisher;
    if (publisher != null)
        await publisher.InitializeAsync();
}

app.Run();