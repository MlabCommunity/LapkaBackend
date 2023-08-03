using Azure.Storage;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Sas;
using LapkaBackend.Application.Common;
using LapkaBackend.Application.Exceptions;
using LapkaBackend.Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace LapkaBackend.Infrastructure.FileStorage;

public class AzureStorageContext : IAzureStorageContext
{
    private readonly IConfiguration _configuration;
    private readonly BlobServiceClient _blobServiceClient;

    public AzureStorageContext(IConfiguration configuration)
    {
        _configuration = configuration;
        _blobServiceClient = new BlobServiceClient(configuration.GetValue<string>("Storage:ConnectionString"));
    }
    
    public async Task<string> GetFileUrlAsync(string fileName)
    {
        var containers = await Task.WhenAll(SearchContainer("lappka-img", fileName), 
            SearchContainer("lappka-others", fileName));

        if (containers.All(x => x is null))
        {
            throw new NotFoundException("invalid_id", "File does not exist");
        }

        var blobClient = containers.First(x => x is not null);

        var sasBuilder = new BlobSasBuilder
        {
            BlobContainerName = blobClient.BlobContainerName,
            BlobName = fileName,
            Resource = "b",
            StartsOn = DateTimeOffset.UtcNow,
            ExpiresOn = DateTimeOffset.UtcNow.AddMinutes(10)
        };

        sasBuilder.SetPermissions(BlobSasPermissions.Read);

        var sasToken = sasBuilder.ToSasQueryParameters(new StorageSharedKeyCredential(_blobServiceClient.AccountName,
            _configuration.GetValue<string>("Storage:Key")));

        return blobClient.Uri.AbsoluteUri + "?" + sasToken;
    }
    
    public async Task UploadFileAsync(IFormFile file, string containerName, string fileName)
    {
        var fileStream = new MemoryStream();

        await file.CopyToAsync(fileStream);

        fileStream.Seek(0, SeekOrigin.Begin);

        await CreateContainerIfNotExists(containerName);
        
        var blobContainer = _blobServiceClient.GetBlobContainerClient(containerName);

        await blobContainer.UploadBlobAsync(fileName, fileStream);

        var blobClient = blobContainer.GetBlobClient(fileName);
        var blobHttpHeaders = new BlobHttpHeaders
        {
            ContentType = file.ContentType
        };
        await blobClient.SetHttpHeadersAsync(blobHttpHeaders);
    }

    public async Task DeleteFileAsync(FileBlob file)
    {
        var containers = await Task.WhenAll(SearchContainer("lappka-img", file.BlobName), SearchContainer("lappka-others", file.BlobName));

        if (containers.All(x => x is null))
        {
            throw new NotFoundException("invalid_id", "File does not exist");
        }

        var blobClient = containers.First(x => x is not null);
        
        await blobClient.DeleteAsync();
    }
    
    private async Task<BlobClient> SearchContainer(string containerName, string fileName)
    {
        await CreateContainerIfNotExists(containerName);
        var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
        var blobClient = containerClient.GetBlobClient(fileName);

        return await blobClient.ExistsAsync() ? blobClient : null!;
    }
    
    private async Task CreateContainerIfNotExists(string containerName)
    {
        var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);

        await containerClient.CreateIfNotExistsAsync();
    }
}