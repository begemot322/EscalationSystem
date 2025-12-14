using EscalationService.Domain.Entities;

namespace EscalationService.Domain;

public class NotificationChannel
{
    public int Id { get; set; }         // PK
    public string Code { get; set; }    // "EMAIL", "PUSH", "SMS", "IN_APP"
    public string Name { get; set; }    // "Электронная почта", "Push-уведомление", и т.д.

    // Связь с Notification (один ко многим)
    public ICollection<Notification> Notifications { get; set; } = new List<Notification>();
}