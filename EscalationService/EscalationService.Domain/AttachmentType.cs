using EscalationService.Domain.Entities;

namespace EscalationService.Domain;

public class AttachmentType
{
    public int Id { get; set; }         // PK
    public string Code { get; set; }    // например: "IMAGE", "PDF", "LOG"
    public string Name { get; set; }    // например: "Изображение", "PDF-документ", "Лог-файл"

    // Связь с Attachment (один ко многим)
    public ICollection<Attachment> Attachments { get; set; } = new List<Attachment>();
}