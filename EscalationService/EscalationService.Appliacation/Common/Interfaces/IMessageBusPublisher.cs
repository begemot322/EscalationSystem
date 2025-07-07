namespace EscalationService.Appliacation.Common.Interfaces;

public interface IMessageBusPublisher
{
    Task PublishUserIds(List<int> userIds);
}