namespace Models.DTOs;

public class EscalationDto
{
    public string Name { get; set; }
    public string Description { get; set; }
    public EscalationStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}