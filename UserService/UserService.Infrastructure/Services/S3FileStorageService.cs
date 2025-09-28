using Amazon.S3;
using Amazon.S3.Model;
using Models.Result;
using UserService.Application.Common.Interfaces;
using UserService.Application.Models.DTOs;
using UserService.Infrastructure.Constants;

namespace UserService.Infrastructure.Services;

public class S3FileStorageService : IFileStorageService
{
    private readonly IAmazonS3 _s3Client;
    
    public S3FileStorageService(IAmazonS3 s3Client)
    {
        _s3Client = s3Client;
    }
    
    public async Task<Result<string>> UploadFileAsync(FileUploadDto fileUploadDto, string? folder = null)
    {
        if (string.IsNullOrEmpty(fileUploadDto.FileName))
            return Result<string>.Failure(Error.ValidationFailed("File name is required"));
        
        var key = GetFullKey(fileUploadDto.FileName, folder);
        
        var putRequest = new PutObjectRequest
        {
            BucketName = StorageConstants.UserBucketName,
            Key = key,
            InputStream = fileUploadDto.InputStream,
            ContentType = fileUploadDto.ContentType
        };

        await _s3Client.PutObjectAsync(putRequest);
        return Result<string>.Success(key);
    }
    
    public async Task<Result<bool>> DeleteFileAsync(string fileName, string? folder = null)
    {
        if (string.IsNullOrEmpty(fileName))
            return Result<bool>.Failure(Error.ValidationFailed("File name is required"));
        
        var key = GetFullKey(fileName, folder);
        
        try
        {
            await _s3Client.DeleteObjectAsync(StorageConstants.UserBucketName, key);
            return Result<bool>.Success(true);
        }
        catch (AmazonS3Exception ex) 
        {
            return Result<bool>.Failure(Error.InternalServerError($"Amazon S3 error: {ex.Message}"));
        }
    }
    
    
    private string GetFullKey(string fileName, string? folder)
    {
        return string.IsNullOrEmpty(folder) 
            ? fileName 
            : $"{folder.Trim('/')}/{fileName}";
    }
}