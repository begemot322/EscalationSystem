using Models.Result;
using UserService.Application.Common.Interfaces;
using UserService.Application.Common.Interfaces.Repository;
using UserService.Application.Models.DTOs;
using UserService.Application.Services.Interfaces;
using UserService.Domain;

namespace UserService.Application.Services.Implementation;

public class UserImageService(
    IFileStorageService fileStorage,
    IUserRepository userRepository,
    IUserContext userContext)
    : IUserImageService
{
    private readonly IFileStorageService _fileStorage = fileStorage;
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IUserContext _userContext = userContext;
    
    private const string AvatarFolder = "avatars";
    private const string AvatarFileNameTemplate = "user_{0}";

    public async Task<Result<string>> UploadUserAvatarAsync(FileUploadDto fileUpload)
    {
        if (fileUpload?.InputStream == null || fileUpload.InputStream.Length == 0)
            return Result<string>.Failure(Error.ValidationFailed("File is required"));
        
        var userId = _userContext.GetUserId();
        
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null)
            return Result<string>.Failure(Error.NotFound<User>(userId));
      
        if (!string.IsNullOrEmpty(user.ProfileImageFileName))
        {
            await _fileStorage.DeleteFileAsync(user.ProfileImageFileName, AvatarFolder);
        }
        
        var fileExtension = Path.GetExtension(fileUpload.FileName);
        var avatarFileName = string.Format(AvatarFileNameTemplate, userId) + fileExtension;
        
        var uploadDto = new FileUploadDto
        {
            FileName = avatarFileName,
            InputStream = fileUpload.InputStream,
            ContentType = fileUpload.ContentType
        };
        
        var uploadResult = await _fileStorage.UploadFileAsync(uploadDto, AvatarFolder);
        
        if (uploadResult.IsSuccess)
        {
            user.ProfileImageFileName = avatarFileName;
            await _userRepository.UpdateAsync(user);
        }

        return uploadResult;
    }
    
    public async Task<Result<bool>> DeleteUserAvatarAsync()
    {
        var userId = _userContext.GetUserId();
        
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null)
            return Result<bool>.Failure(Error.NotFound<User>(userId));

        if (string.IsNullOrEmpty(user.ProfileImageFileName))
            return Result<bool>.Success(true);

        var deleteResult = await _fileStorage.DeleteFileAsync(user.ProfileImageFileName, AvatarFolder);
        
        if (deleteResult.IsSuccess)
        {
            user.ProfileImageFileName = null;
            await _userRepository.UpdateAsync(user);
        }

        return deleteResult;
    }
}