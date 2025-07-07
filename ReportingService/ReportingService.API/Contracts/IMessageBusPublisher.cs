using Models.DTOs;

namespace ReportingService.API.Contracts;

public interface IMessageBusPublisher
{
    Task<List<EscalationDtoMessage>> GetEscalationsAsync(ReportRequest request);

}