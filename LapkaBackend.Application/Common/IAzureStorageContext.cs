using LapkaBackend.Domain.Entities;
using Microsoft.AspNetCore.Http;

namespace LapkaBackend.Application.Common;

public interface IAzureStorageContext
{
    Task<string> GetFileUrlAsync(string fileName);
    Task UploadFileAsync(IFormFile file, Guid parentId, string containerName, string fileName, Guid? updateId = null);
    Task DeleteFileAsync(FileBlob file);
}