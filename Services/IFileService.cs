using AwsS3.Shared;

namespace AwsS3.Services
{
    public interface IFileService
    {
        Task<Result> UploadFileAsync(IFormFile file, string bucketName, string? prefix);
        Task<Result> GetAllFilesAsync(string bucketName, string? prefix);
        Task<Result<(Stream FileStream, string ContentType)>> GetFileByKeyAsync(string bucketName, string key);
        Task<Result> DeleteFileAsync(string bucketName, string key);
    }
}
