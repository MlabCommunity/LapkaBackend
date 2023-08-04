using Microsoft.AspNetCore.Http;

namespace LapkaBackend.Application.Interfaces;

public interface IBlobService
{
    Task<string> UploadFileAsync(IFormFile file, Guid parentId, string containerName, Guid? id = null);
    Task<List<string>> UploadFilesAsUserAsync(IFormFile[] file);
    Task<List<string>> UploadFilesAsShelterAsync(IFormFile[] file, Guid parentId);
    Task DeleteFileAsync(Guid id);
    Task<string> GetFileUrlAsync(Guid id);
    Task UpdateFileAsUserAsync(IFormFile file, Guid pictureId);
    Task UpdateFileAsShelterAsync(IFormFile file, Guid id);
    Task DeleteListOfFiles(List<string> ids);
    Task UpdateFileName(Guid id, string newName, Guid userId);
}