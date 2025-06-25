namespace EscalationService.Domain.Entities;

public class EscalationUser
{
    public int EscalationId { get; set; }
    public int UserId { get; set; }
    
    public Escalation Escalation { get; set; }
}