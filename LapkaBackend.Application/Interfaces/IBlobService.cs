using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.AspNetCore.Http;

namespace LapkaBackend.Application.Interfaces;

public interface IBlobService
{
    Task<string> UploadFileAsync(IFormFile file);
    Task DeleteFileAsync(Guid id);
    Task<string> GetFileUrlAsync(Guid id);
}