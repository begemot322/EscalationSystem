namespace EscalationService.Domain.Entities;

public class Criteria
{
    public int Id { get; set; }
    public string Description { get; set; }
    /// <summary>
    /// Порядковый номер критерия
    /// </summary>
    public int Order { get; set; }
    public bool IsCompleted { get; set; }
    public int EscalationId { get; set; }
}