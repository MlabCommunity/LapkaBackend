﻿using System.Text.RegularExpressions;
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
    private readonly IAzureStorageContext _storageContext;
    private readonly IDataContext _dbContext;
    private readonly int _imageFileMaxSize;
    private readonly int _otherFileMaxSize;

    public BlobService(IAzureStorageContext storageContext, IDataContext dataContext, IConfiguration configuration)
    {
        _storageContext = storageContext;
        _dbContext = dataContext;
        _imageFileMaxSize = configuration.GetValue<int>("FileLimits:Image");
        _otherFileMaxSize = configuration.GetValue<int>("FileLimits:Other");
    }

    public async Task<string> GetFileUrlAsync(Guid id)
    {
        var file = await _dbContext.Blobs.FirstOrDefaultAsync(x => x.Id.Equals(id));

        if (file is null)
        {
            throw new NotFoundException("invalid_id", "File does not exist");
        }

        return await _storageContext.GetFileUrlAsync(file.BlobName);
    }

    public async Task<string> UploadFileAsync(IFormFile file, Guid parentId, string containerName, Guid? updateId = null)
    {
        var blob = new FileBlob()
        {
            Id = updateId ?? Guid.NewGuid(),
            UploadName = file.FileName,
            BlobName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName),
            FileType = file.ContentType,
            ParentEntityId = parentId
        };

        await _dbContext.Blobs.AddAsync(blob);
        await _dbContext.SaveChangesAsync();
        await _storageContext.UploadFileAsync(file, parentId, containerName, blob.BlobName);

        return blob.Id.ToString();
    }

    public async Task DeleteFileAsync(Guid id)
    {
        var file = await _dbContext.Blobs.FirstOrDefaultAsync(x => x.Id.Equals(id));

        if (file is null)
        {
            throw new NotFoundException("invalid_id", "File does not exist");
        }

        var user = await _dbContext.Users.FirstOrDefaultAsync(x => x.ProfilePicture.Equals(file.Id.ToString()));
        
        if (user is not null)
        {
            user.ProfilePicture = string.Empty;
            _dbContext.Users.Update(user);
            await _dbContext.SaveChangesAsync();
        }
        
        _dbContext.Blobs.Remove(file);
        await _dbContext.SaveChangesAsync();
        await _storageContext.DeleteFileAsync(file);
    }

    public async Task<string> UploadFileAsUserAsync(IFormFile file, Guid parentId)
    {
        if (file.Length > _imageFileMaxSize)
        {
            throw new BadRequestException("invalid_file", "File too large, Exceeded 5MB");
        }
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
        if (file.Length > _otherFileMaxSize)
        {
            throw new BadRequestException("invalid_file", "File too large, Exceeded 15MB");
        }
        
        return await UploadFileAsync(file, parentId, "lappka-others");

    }

    public async Task UpdateFileAsUserAsync(IFormFile file, Guid pictureId)
    {
        if (file.Length > _imageFileMaxSize)
        {
            throw new BadRequestException("invalid_file", "File too large, Exceeded 5MB");
        }
        
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
        if (file.Length > _otherFileMaxSize)
        {
            throw new BadRequestException("invalid_file", "File too large, Exceeded 15MB");
        }

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

    private readonly List<string> _pictureTypes = new()
    {
        "image/gif",
        "image/jpeg",
        "image/png",
        "image/bmp"
    };
}