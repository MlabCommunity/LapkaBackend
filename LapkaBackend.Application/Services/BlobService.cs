using Azure.Storage;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Sas;
using LapkaBackend.Application.Common;
using LapkaBackend.Application.Exceptions;
using LapkaBackend.Application.Interfaces;
using LapkaBackend.Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;


namespace LapkaBackend.Application.Services;

public class BlobService : IBlobService
{
    private readonly BlobServiceClient _blobServiceClient;
    private readonly IConfiguration _configuration;
    private readonly IDataContext _dbContext;
    

    public BlobService(BlobServiceClient blobServiceClient, IConfiguration configuration, IDataContext dataContext)
    {
        _blobServiceClient = blobServiceClient;
        _configuration = configuration;
        _dbContext = dataContext;
    }
    
    public async Task<string> GetFileUrlAsync(Guid id)
    {
        var fileName = (await _dbContext.Blobs.FirstAsync(x => x.Id.Equals(id))).BlobName;

        var searchingContainers = new List<Task<BlobClient>>();

        foreach(var container in _blobServiceClient.GetBlobContainers()) 
        {
            searchingContainers.Add(Task.Run(() => SearchContainer(container.Name, fileName)));
        }

        var containers = await Task.WhenAll(searchingContainers);

        if (containers.All(x => x is null))
        {
            throw new NotFoundException("invalid_id", "File does not exist");
        }

        var blobClient = containers.First(x => x is not null);

        BlobSasBuilder sasBuilder = new BlobSasBuilder
        {
            BlobContainerName = blobClient.BlobContainerName,
            BlobName = fileName,
            Resource = "b",
            StartsOn = DateTimeOffset.UtcNow,
            ExpiresOn = DateTimeOffset.UtcNow.AddMinutes(10)
        };

        sasBuilder.SetPermissions(BlobSasPermissions.Read);

        var sasToken = sasBuilder.ToSasQueryParameters(new StorageSharedKeyCredential(_blobServiceClient.AccountName, _configuration.GetConnectionString("AzureKey"))).ToString();
        
        return blobClient.Uri.AbsoluteUri + "?" + sasToken;
    }

    public async Task<string> UploadFileAsync(IFormFile file)
    {
        var fileStream = new MemoryStream();

        await file.CopyToAsync(fileStream);

        fileStream.Seek(0, SeekOrigin.Begin);

        FileBlob blob = new FileBlob()
        {
            UploadName = file.FileName,
            BlobName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName),
            FileType = file.ContentType,
            ParentEntityId = Guid.NewGuid()
            //TODO: Podmienić na Shelter/Pets/User Id w przyszłości 
        };
         
        await _dbContext.Blobs.AddAsync(blob);
        await _dbContext.SaveChangesAsync();

        var blobContainer = _blobServiceClient.GetBlobContainerClient("lappka-img");
        
        await blobContainer.UploadBlobAsync(blob.BlobName, fileStream);

        var blobClient = blobContainer.GetBlobClient(blob.BlobName);
        BlobHttpHeaders blobHttpHeaders = new BlobHttpHeaders();
        blobHttpHeaders.ContentType = file.ContentType;
        blobClient.SetHttpHeaders(blobHttpHeaders);

        return blob.Id.ToString();
    }

    public async Task DeleteFileAsync(Guid id)
    {
        var file = await _dbContext.Blobs.FirstAsync(x => x.Id.Equals(id));

        var searchedContainer = new List<Task<BlobClient>>();

        foreach (var container in _blobServiceClient.GetBlobContainers())
        {
            searchedContainer.Add(Task.Run(() => SearchContainer(container.Name, file.BlobName)));
        }

        var containers = await Task.WhenAll(searchedContainer);

        if (containers.All(x => x is null))
        {
            throw new NotFoundException("invalid_id", "File does not exist");
        }

        var blobClient = containers.First(x => x is not null);

        _dbContext.Blobs.Remove(file);
        await _dbContext.SaveChangesAsync();
        await blobClient.DeleteAsync();
    }

    private BlobClient SearchContainer(string containerName, string fileName)
    {
        var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
        var blobClient = containerClient.GetBlobClient(fileName);

        return blobClient.Exists() ? blobClient : null!;
    }
}