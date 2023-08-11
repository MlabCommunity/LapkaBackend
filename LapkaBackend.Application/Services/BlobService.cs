using System.Reflection.Metadata;
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
    
    public async Task<List<string>> GetFilesUrlsAsync(List<Guid> ids)
    {
        var links = new List<string>();
        foreach (var id in ids)
        {
            var file = await _dbContext.Blobs.FirstOrDefaultAsync(x => x.Id.Equals(id));

            if (file is null)
            {
                throw new NotFoundException("invalid_id", "File does not exist");
            }
            links.Add(await _storageContext.GetFileUrlAsync(file.BlobName));
        }

        return links;
    }

    public async Task<string> UploadFileAsync(IFormFile file, Guid parentId, string containerName, Guid? updateId = null)
    {
        var blob = new FileBlob()
        {
            Id = updateId ?? Guid.NewGuid(),
            UploadName = file.FileName,
            BlobName = Guid.NewGuid() + Path.GetExtension(file.FileName),
            FileType = file.ContentType,
            ParentEntityId = parentId
        };

        await _dbContext.Blobs.AddAsync(blob);
        await _dbContext.SaveChangesAsync();
        await _storageContext.UploadFileAsync(file, containerName, blob.BlobName);

        return blob.Id.ToString();
    }

    public async Task DeleteFileAsync(Guid id)
    {
        var file = await _dbContext.Blobs.FirstOrDefaultAsync(x => x.Id.Equals(id));

        if (file is null)
        {
            throw new NotFoundException("invalid_id", "File does not exist");
        }

        var user = await _dbContext.Users.FirstOrDefaultAsync(x => x.ProfilePicture!.Equals(file.Id.ToString()));
        
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

    public async Task<List<string>> UploadFilesAsUserAsync(List<IFormFile> files)
    {
        if (files.Any((x => x.Length > _imageFileMaxSize)))
        {
            throw new BadRequestException("invalid_file", "File too large, Exceeded 5MB");
        }

        if (files.Any(x => !_pictureTypes.Contains(x.ContentType)))
        {
            throw new BadRequestException("invalid_image", "Format of image is invalid");
        }
        
        var idFileList = new List<string>();
        
        foreach (var file in files)
        {
            idFileList.Add(await UploadFileAsync(file, Guid.Empty, "lappka-img"));
        }

        return idFileList;
    }

    public async Task<List<string>> UploadFilesAsShelterAsync(List<IFormFile> files, Guid parentId)
    {
        if (files.Any(x => x.Length > _otherFileMaxSize))
        {
            throw new BadRequestException("invalid_file", "One of the files is too large, Exceeded 15MB");
        }

        var idFileList = new List<string>();
        foreach (var file in files)
        {
            idFileList.Add(await UploadFileAsync(file, parentId, "lappka-others"));
        }

        return idFileList;
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

        if (blobFile.ParentEntityId != userId)
        {
            throw new ForbiddenException("invalid_user", "You are not allowed to modify this file");
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

    public async Task DeleteFilesByParentId(Guid id)
    {
        var results = _dbContext.Blobs.Where(x => x.ParentEntityId == id)
            .Select(blb => blb.Id.ToString());

        await DeleteListOfFiles(results.ToList());
    }

    private readonly List<string> _pictureTypes = new()
    {
        "image/gif",
        "image/jpeg",
        "image/png",
        "image/bmp"
    };
}