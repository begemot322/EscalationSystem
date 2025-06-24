namespace EscalationService.Domain.Entities;

public class Comment
{
    public int Id { get; set; }
    public string Text { get; set; }
    public int AuthorId { get; set; } 
    public int EscalationId { get; set; } 
}