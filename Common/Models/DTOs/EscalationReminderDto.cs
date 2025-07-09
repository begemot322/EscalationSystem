namespace Models.DTOs;

public class EscalationReminderDto
{
    public int Id { get; set; }  
    public string Name { get; set; }
    public int AuthorId { get; set; }     
    public List<int> ResponsibleUserIds { get; set; } = new();
}