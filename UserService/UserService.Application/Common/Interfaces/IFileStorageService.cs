using Models.Result;
using UserService.Application.Models.DTOs;

namespace UserService.Application.Common.Interfaces;

public interface IFileStorageService
{
    Task<Result<string>> UploadFileAsync(FileUploadDto fileUploadDto, string? folder = null);
    Task<Result<bool>> DeleteFileAsync(string fileName, string? folder = null);
}