using Microsoft.AspNetCore.Mvc;
using NotificationService.API;
using NotificationService.API.Contracts;
using NotificationService.API.Models;

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

app.MapPost("/email", async ([FromBody] SendEmailRequest sendEmailRequest,
    IEmailService mailService) =>
{
    await mailService.SendEmailAsync(sendEmailRequest);
    return Results.Ok("Проверка email");
});


app.MapControllers();

app.Run();