using EscalationService.Domain.Enums;

namespace EscalationService.Domain.Entities;

public class Escalation
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public EscalationStatus Status { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public int AuthorId { get; set; } 
    
}