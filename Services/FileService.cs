using AwsS3.Shared;
using Amazon.S3;
using Amazon.S3.Model;
using AwsS3.DTOs;

namespace AwsS3.Services
{
    public class FileService : IFileService
    {
        private readonly IAmazonS3 _amazonS3;

        public FileService(IAmazonS3 amazonS3) => _amazonS3 = amazonS3;

        public async Task<Result> UploadFileAsync(IFormFile file, string bucketName, string? prefix)
        {
            if (file.Length == 0) return Result.Fail("No file uploaded.");

            var bucketExists = await Amazon.S3.Util.AmazonS3Util.DoesS3BucketExistV2Async(_amazonS3, bucketName);
            if (!bucketExists) return Result.Fail($"Bucket {bucketName} not found.");

            PutObjectRequest request = new()
            {
                BucketName = bucketName,
                Key = string.IsNullOrEmpty(prefix) ? file.FileName : $"{prefix?.TrimEnd('/')}/{file.FileName}",
                InputStream = file.OpenReadStream(),
            };

            request.Metadata.Add("Content-Type", file.ContentType);
            request.Metadata.Add("Uploaded-Date", DateTime.Now.ToString());

            await _amazonS3.PutObjectAsync(request);
            return Result.Ok($"File {file.FileName} uploaded successfully!");
        }

        public async Task<Result> DeleteFileAsync(string bucketName, string key)
        {
            var bucketExists = await Amazon.S3.Util.AmazonS3Util.DoesS3BucketExistV2Async(_amazonS3, bucketName);
            if (!bucketExists) return Result.Fail($"Bucket {bucketName} not found.");

            await _amazonS3.DeleteObjectAsync(bucketName, key);
            return Result.Ok();
        }

        public async Task<Result> GetAllFilesAsync(string bucketName, string? prefix)
        {
            var bucketExists = await Amazon.S3.Util.AmazonS3Util.DoesS3BucketExistV2Async(_amazonS3, bucketName);
            if (!bucketExists) return Result.Fail($"Bucket {bucketName} not found.");

            var reqest = new ListObjectsV2Request
            {
                BucketName = bucketName,
                Prefix = prefix
            };

            var result = await _amazonS3.ListObjectsV2Async(reqest);
            var s3Objects = result.S3Objects.Select(s =>
            {
                var urlRequest = new GetPreSignedUrlRequest
                {
                    BucketName = bucketName,
                    Key = s.Key,
                    Expires = DateTime.Now.AddMinutes(1)
                };

                return new S3ObjectDTO
                {
                    Name = s.Key.ToString(),
                    PresignedUrl = _amazonS3.GetPreSignedURL(urlRequest)
                };
            });
            return Result.Ok(s3Objects);
        }

        public async Task<Result<(Stream FileStream, string ContentType)>> GetFileByKeyAsync(string bucketName, string key)
        {
            var bucketExists = await Amazon.S3.Util.AmazonS3Util.DoesS3BucketExistV2Async(_amazonS3, bucketName);
            if (!bucketExists)
                return Result.Fail<(Stream, string)>($"Bucket {bucketName} not found.");

            var s3Object = await _amazonS3.GetObjectAsync(bucketName, key);

            return Result.Ok((s3Object.ResponseStream, s3Object.Headers.ContentType));

        }
    }
}