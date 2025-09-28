using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Util;
using UserService.Infrastructure.Constants;

namespace UserService.Infrastructure.Services;

public class BucketInitializationService
{
    private readonly IAmazonS3 _s3Client;

    public BucketInitializationService(IAmazonS3 s3Client)
    {
        _s3Client = s3Client;
    }
    public async Task InitializeBucketAsync()
    {
        var bucketExists = await AmazonS3Util.DoesS3BucketExistV2Async(_s3Client, StorageConstants.UserBucketName);
        if (!bucketExists)
        {
            await _s3Client.PutBucketAsync(new PutBucketRequest
            {
                BucketName = StorageConstants.UserBucketName,
            });
        }
    }
}