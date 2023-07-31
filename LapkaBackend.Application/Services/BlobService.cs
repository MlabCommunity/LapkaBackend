using System.Text.RegularExpressions;
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
using MimeTypes;


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
        var file = await _dbContext.Blobs.FirstOrDefaultAsync(x => x.Id.Equals(id));

        if (file is null)
        {
            throw new NotFoundException("invalid_id", "File does not exist");
        }

        var fileName = file.BlobName;

        var searchingContainers = _blobServiceClient.GetBlobContainers()
            .Select(container => Task.Run(() => SearchContainer(container.Name, fileName))).ToList();

        var containers = await Task.WhenAll(searchingContainers);

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

        var sasToken = sasBuilder.ToSasQueryParameters(new StorageSharedKeyCredential(_blobServiceClient.AccountName, _configuration.GetConnectionString("AzureKey"))).ToString();

        return blobClient.Uri.AbsoluteUri + "?" + sasToken;
    }

    public async Task<string> UploadFileAsync(IFormFile file, Guid parentId, string containerName, Guid? updateId = null)
    {
        var fileStream = new MemoryStream();

        await file.CopyToAsync(fileStream);

        fileStream.Seek(0, SeekOrigin.Begin);

        FileBlob blob = new FileBlob()
        {
            Id = updateId == null ? Guid.NewGuid() : (Guid)updateId,
            UploadName = file.FileName,
            BlobName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName),
            FileType = file.ContentType,
            ParentEntityId = parentId
        };

        await _dbContext.Blobs.AddAsync(blob);
        await _dbContext.SaveChangesAsync();

        var blobContainer = _blobServiceClient.GetBlobContainerClient(containerName);

        await blobContainer.UploadBlobAsync(blob.BlobName, fileStream);

        var blobClient = blobContainer.GetBlobClient(blob.BlobName);
        BlobHttpHeaders blobHttpHeaders = new BlobHttpHeaders();
        blobHttpHeaders.ContentType = file.ContentType;
        blobClient.SetHttpHeaders(blobHttpHeaders);
        
        return blob.Id.ToString();
    }

    public async Task DeleteFileAsync(Guid id)
    {
        var file = await _dbContext.Blobs.FirstOrDefaultAsync(x => x.Id.Equals(id));

        if (file is null)
        {
            throw new NotFoundException("invalid_id", "File does not exist");
        }

        var searchedContainers = _blobServiceClient.GetBlobContainers()
             .Select(container => Task.Run(() => SearchContainer(container.Name, file.BlobName))).ToList();

        var containers = await Task.WhenAll(searchedContainers);

        if (containers.All(x => x is null))
        {
            throw new NotFoundException("invalid_id", "File does not exist");
        }

        var blobClient = containers.First(x => x is not null);

        _dbContext.Blobs.Remove(file);
        await _dbContext.SaveChangesAsync();
        await blobClient.DeleteAsync();
    }

    public async Task<string> UploadFileAsUserAsync(IFormFile file, Guid parentId)
    {
        var user = await _dbContext.Users.FirstAsync(x => x.Id == parentId);

        if (!_pictureTypes.Contains(file.ContentType))
        {
            throw new BadRequestException("invalid_image", "Format of image is invalid");
        }

        var pictureId = await UploadFileAsync(file, parentId, "lappka-img");
        user.ProfilePicture = pictureId;

        _dbContext.Users.Update(user);
        await _dbContext.SaveChangesAsync();
        return pictureId;
    }

    public async Task<string> UploadFileAsShelterAsync(IFormFile file, Guid parentId)
    {
        if (_pictureTypes.Contains(file.ContentType))
        {
            if (file.Length >= 5242880)
            {
                throw new BadRequestException("invalid_image", "Image is too large (Exceeded 5MB)");
            }

            return await UploadFileAsync(file, parentId, "lappka-img");
        }
        return await UploadFileAsync(file, parentId, "lappka-others");

    }

    private BlobClient SearchContainer(string containerName, string fileName)
    {
        var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
        var blobClient = containerClient.GetBlobClient(fileName);

        return blobClient.Exists() ? blobClient : null!;
    }

    public async Task UpdateFileAsUserAsync(IFormFile file, Guid pictureId)
    {

        var oldFile = await _dbContext.Blobs.FirstOrDefaultAsync(x => x.Id == pictureId);

        if (oldFile is null)
        {
            throw new NotFoundException("invalid_id", "File does not exists");
        }

        if (!_pictureTypes.Contains(file.ContentType))
        {
            throw new BadRequestException("invalid_image", "Format of image is invalid");
        }

        await DeleteFileAsync(pictureId);

        await UploadFileAsync(file, oldFile.ParentEntityId, "lappka-img", pictureId);
    }

    public async Task UpdateFileAsShelterAsync(IFormFile file, Guid id)
    {
        var oldFile = await _dbContext.Blobs.FirstOrDefaultAsync(x => x.Id == id);

        if (oldFile is null)
        {
            throw new NotFoundException("invalid_id", "File does not exists");
        }

        await DeleteFileAsync(id);

        await UploadFileAsync(file, oldFile.ParentEntityId, "lappka-others", id);
    }

    public async Task UpdateFileName(Guid id, string newName, Guid userId)
    {
        var blobFile = await _dbContext.Blobs.FirstOrDefaultAsync(x => x.Id == id);

        if (blobFile is null)
        {
            throw new NotFoundException("invalid_id", "File does not exists");
        }

        if (!(blobFile.ParentEntityId == userId))
        {
            throw new ForbiddenExcpetion("invalid_user", "You are not allowed to modify this file");
        }

        blobFile.UploadName = newName + Path.GetExtension(blobFile.UploadName);

        _dbContext.Blobs.Update(blobFile);
        await _dbContext.SaveChangesAsync();
    }
    

    public async Task DeleteListOfFiles(List<string> filesIds)
    {
        foreach (var file in filesIds)
        {
            await DeleteFileAsync(new Guid(file));
        }
    }

    public Task<FormFile> ConvertByte64ToFile(string fileString, string fileName)
    {
        var base64FileString = fileString.Split(',')[1];
        byte[] fileBytes = Convert.FromBase64String(base64FileString);
        var fileMemoryStream = new MemoryStream(fileBytes);
        // return fileMemoryStream;
        string header = Regex.Match(fileString, @"[^:]\w+\/[\w-+\d.]+(?=;|,)").Value;
        var formFile = new FormFile(fileMemoryStream, 0, fileMemoryStream.Length, null!, fileName + MimeTypeMap.GetExtension(header))
            {
                Headers = new HeaderDictionary(),
                ContentType = header
            };
        return Task.FromResult(formFile);
    }

    private readonly List<string> _pictureTypes = new()
    {
        "image/gif",
        "image/jpeg",
        "image/png",
        "image/bmp"
    };
}