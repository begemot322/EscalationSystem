using Models.Result;
using UserService.Application.Models.DTOs;

namespace UserService.Application.Services.Interfaces;

public interface IUserImageService
{
    // Тут пока аватарки
    Task<Result<string>> UploadUserAvatarAsync(FileUploadDto fileUpload);
    // Task<Result<Stream>> GetUserAvatarAsync();
    Task<Result<bool>> DeleteUserAvatarAsync();
    
    // В будущем можно будет просто добавлять не ток авы
}