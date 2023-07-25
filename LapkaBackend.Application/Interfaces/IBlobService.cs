using Azure;
using Azure.Storage.Blobs.Models;

namespace LapkaBackend.Application.Interfaces;

public interface IBlobService
{
    // public Task UploadFileAsync(string filePath, string fileName);
    // public Task DeleteFileAsync(string fileName);
    public Task<string> GetFileUrlAsync(string fileName);
}