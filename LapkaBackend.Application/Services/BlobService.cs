using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using LapkaBackend.Application.Interfaces;

namespace LapkaBackend.Application.Services;

public class BlobService : IBlobService
{
    private readonly BlobServiceClient _blobServiceClient;

    public BlobService(BlobServiceClient blobServiceClient)
    {
        _blobServiceClient = blobServiceClient;
    }
    
    public async Task<string> GetFileUrlAsync(string fileName)
    {
        var containerClient = _blobServiceClient.GetBlobContainerClient("lappka");
        var blobClient = containerClient.GetBlobClient(fileName);
        var blobUrl = blobClient.Uri.AbsoluteUri;
        return blobClient.Uri.AbsoluteUri;
        // url do pliku
    }
    
}