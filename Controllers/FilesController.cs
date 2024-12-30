using AwsS3.Services;
using Microsoft.AspNetCore.Mvc;

namespace AwsS3.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FilesController : ControllerBase
    {
        IFileService _fileService;

        public FilesController(IFileService fileService)
        {
            _fileService = fileService;
        }

        [HttpPost]
        [Route("upload")]
        public async Task<IActionResult> UploadFileAsync(IFormFile file, string bucketName, string? prefix)
        {
            var result = await _fileService.UploadFileAsync(file, bucketName, prefix);
            if (!result.IsSuccess)
                return BadRequest(result.Error);

            return Ok("File uploaded successfully.");
        }

        [HttpGet]
        [Route("get-all-files")]
        public async Task<IActionResult> GetAllFilesAsync(string bucketName, string? prefix)
        {
            var result = await _fileService.GetAllFilesAsync(bucketName, prefix);
            if (!result.IsSuccess)
                return BadRequest(result.Error);

            return Ok(result);
        }

        [HttpGet]
        [Route("download")]
        public async Task<IActionResult> GetFileByKeyAsync(string bucketName, string key)
        {
            var result = await _fileService.GetFileByKeyAsync(bucketName, key);
            if (!result.IsSuccess)
                return BadRequest(result.Error);

            var (fileStream, contentType) = result.Value;
            return File(fileStream, contentType, key);
        }

        [HttpDelete]
        [Route("delete")]
        public async Task<IActionResult> DeleteFileAsync(string bucketName, string key)
        {
            var result = await _fileService.DeleteFileAsync(bucketName, key);
            if (!result.IsSuccess)
                return BadRequest(result.Error);

            return Ok("File deleted successfully.");
        }
    }
}
