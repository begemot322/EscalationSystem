namespace UserService.Application.Models.DTOs;

public class FileUploadDto
{
    public string FileName { get; set; } = null!;
    public Stream InputStream { get; set; } = null!;
    public string ContentType { get; set; } = null!;
}