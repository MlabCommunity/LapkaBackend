using Azure.Storage;
using Azure.Storage.Blobs;
using Azure.Storage.Sas;
using LapkaBackend.Application.Interfaces;
using Microsoft.Extensions.Configuration;

namespace LapkaBackend.Application.Services;

public class BlobService : IBlobService
{
    private readonly BlobServiceClient _blobServiceClient;
    private readonly IConfiguration _configuration;

    public BlobService(BlobServiceClient blobServiceClient, IConfiguration configuration)
    {
        _blobServiceClient = blobServiceClient;
        _configuration = configuration;
    }
    
    public string GetFileUrlAsync(string fileName)
    {
        var containerClient = _blobServiceClient.GetBlobContainerClient("lappka");
        var blobClient = containerClient.GetBlobClient(fileName);
        BlobSasBuilder sasBuilder = new BlobSasBuilder
        {
            BlobContainerName = "lappka",
            BlobName = fileName,
            Resource = "b",
            StartsOn = DateTimeOffset.UtcNow,
            ExpiresOn = DateTimeOffset.UtcNow.AddMinutes(10)
        };
        //TODO does file even exist?
        sasBuilder.SetPermissions(BlobSasPermissions.Read);
        var sasToken = sasBuilder.ToSasQueryParameters(new StorageSharedKeyCredential("lappka", _configuration.GetConnectionString("AzureKey"))).ToString();
        return blobClient.Uri.AbsoluteUri + "?" + sasToken;
        // url do pliku




    }

}