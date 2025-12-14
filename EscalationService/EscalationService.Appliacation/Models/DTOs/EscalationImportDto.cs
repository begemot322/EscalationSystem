namespace EscalationService.Appliacation.Models.DTOs;

public class EscalationImportDto
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Status { get; set; } = "New";
    public DateTime? CreatedAt { get; set; }
    public bool IsFeatured { get; set; }
    public int AuthorId { get; set; }
    public List<int> UserIds { get; set; } = new(); 
}