using Models.DTOs;
using SchedulerService.API.Contracts;

namespace SchedulerService.API.Services;

public class EscalationServiceClient : IEscalationServiceClient
{
    private readonly HttpClient _httpClient;

    public EscalationServiceClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }
    
    public async Task<List<EscalationReminderDto>> GetOverdueEscalations()
    {
        var response = await _httpClient.GetAsync("/internal/escalation/overdue");
        return await response.Content.ReadFromJsonAsync<List<EscalationReminderDto>>();
    }

}