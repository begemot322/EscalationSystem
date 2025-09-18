using Models;

namespace EscalationService.Domain.Entities;

public class Escalation
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public EscalationStatus Status { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public bool IsFeatured { get; set; } = false;

    public int AuthorId { get; set; } 
    
    // Связь с юзерами(many to many)
    public List<EscalationUser> EscalationUsers { get; set; } = new();
}