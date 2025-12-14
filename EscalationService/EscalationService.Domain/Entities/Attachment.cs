namespace EscalationService.Domain.Entities;

//  файлы, прикреплённые к эскалации 
public class Attachment
{
    public int Id { get; set; }
    public string FileName { get; set; }
    public string FilePath { get; set; }
    public long FileSize { get; set; }
    public DateTime UploadedAt { get; set; } = DateTime.UtcNow;

    public int? EscalationId { get; set; }
    public int AttachmentTypeId { get; set; }  

    public Escalation? Escalation { get; set; }
    public AttachmentType AttachmentType { get; set; } 
}