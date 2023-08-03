using LapkaBackend.Domain.Entities;
using Microsoft.AspNetCore.Http;

namespace LapkaBackend.Application.Common;

public interface IAzureStorageContext
{
    Task<string> GetFileUrlAsync(string fileName);
    Task UploadFileAsync(IFormFile file, string containerName, string fileName);
    Task DeleteFileAsync(FileBlob file);
}