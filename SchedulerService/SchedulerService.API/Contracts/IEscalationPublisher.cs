using Models.DTOs;

namespace SchedulerService.API.Contracts;

public interface IEscalationPublisher
{
    Task PublishOverdueEscalationAsync(EscalationReminderDto escalation);
}