namespace EscalationService.Domain.Entities;

public class Notification
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Message { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public bool IsRead { get; set; } = false;

    public int UserId { get; set; }
    public int? EscalationId { get; set; }
    public int NotificationChannelId { get; set; }  

    public Escalation? Escalation { get; set; }
    public NotificationChannel NotificationChannel { get; set; }  
}