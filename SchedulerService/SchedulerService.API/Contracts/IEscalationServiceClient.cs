using Models.DTOs;

namespace SchedulerService.API.Contracts;

public interface IEscalationServiceClient
{
    Task<List<EscalationReminderDto>> GetOverdueEscalations();
}