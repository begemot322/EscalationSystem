using SchedulerService.API.Contracts;

namespace SchedulerService.API.Services;

public class EscalationCheckerService : BackgroundService
{
    private readonly IEscalationServiceClient _escalationClient;
    private readonly IEscalationPublisher _publisher;
    private readonly TimeSpan _checkInterval = TimeSpan.FromDays(3);
    
    public EscalationCheckerService(
        IEscalationServiceClient escalationClient,
        IEscalationPublisher publisher)
    {
        _escalationClient = escalationClient;
        _publisher = publisher;
    }


    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        stoppingToken.ThrowIfCancellationRequested();
        
        var escalations = await _escalationClient.GetOverdueEscalations();
        
        foreach (var esc in escalations)
        {
            await _publisher.PublishOverdueEscalationAsync(esc);
        }
        
        await Task.Delay(_checkInterval, stoppingToken);
    }
}