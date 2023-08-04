using LapkaBackend.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using LapkaBackend.Application.Exceptions;

namespace LapkaBackend.API.Controllers;

[Route("[controller]")]
[ApiController]
public class StorageController : Controller
{
    private readonly IBlobService _blobService;

    public StorageController(IBlobService blobService)
    {
        _blobService = blobService;
    }
    /// <summary>
    ///     Pobieranie pliku na podstawie identyfikatora
    /// </summary>
    [HttpGet ("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> GetFile(Guid id)
    {
        return Ok(await _blobService.GetFileUrlAsync(id));
    }
    
    /// <summary>
    ///     Zastąpienie pliku o wskazanym Id nowym plikiem do 15MB 
    /// </summary>
    [HttpPut("{id}")]
    [Authorize(Roles = "Shelter, SuperAdmin")]
    [RequestSizeLimit(15728640)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> UpdateFileShelter([Required]IFormFile file, [Required]Guid id)
    {
        await _blobService.UpdateFileAsShelterAsync(file, id);

        return NoContent();
    }
    
    /// <summary>
    ///     Usuwanie pliku o wskazanym Id. Użytkownik może usuwać pliki tylko za pośrednictwem innych serwisów. 
    /// </summary>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> DeleteFile(Guid id)
    {
        await _blobService.DeleteFileAsync(id);

        return NoContent();
    }

    /// <summary>
    ///     Dodawanie pliku 15MB i zwrócenie jego identyfikatora. Dostępne dla schroniska.
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Shelter, SuperAdmin")]
    [RequestSizeLimit(15728640)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> SaveFileByShelter(params IFormFile[] files)
    {
        var user = HttpContext.User.FindFirstValue("userId");
        if (user is null)
        {
            throw new UnauthorizedException("invalid_token", "Invalid token");
        }
        
        return Ok(await _blobService.UploadFilesAsShelterAsync(files, new Guid(user)));
    }

    /// <summary>
    ///     Dodawanie zdjęcia do 5MB (.jpg, .jpeg, .bmp, .png) i zwrócenie jego identyfikatora. Dostępne dla zalogowanego użytkownika.
    /// </summary>
    [HttpPost("picture")]
    [Authorize(Roles = "User, Shelter, Admin, SuperAdmin, Worker")]
    [RequestSizeLimit(5242880)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> SaveFileByUser(params IFormFile[] files)
    {
        var user = HttpContext.User.FindFirstValue("userId");
        if (user is null)
        {
            throw new UnauthorizedException("invalid_token", "Invalid token");
        }
        
        return Ok(await _blobService.UploadFilesAsUserAsync(files));
    }

    /// <summary>
    ///     Zastąpienie zdjęcia o wskazanym Id nowym plikiem do 5MB 
    /// </summary>
    [HttpPut("picture/{id}")]
    [Authorize(Roles = "User, Shelter, Admin, SuperAdmin, Worker")]
    [RequestSizeLimit(5242880)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> UpdateFileUser([Required] IFormFile file, [Required] Guid id)
    {
        await _blobService.UpdateFileAsUserAsync(file, id);

        return NoContent();
    }
    
    /// <summary>
    ///     Edytowanie nazwy pliku o wskazanym Id 
    /// </summary>
    [HttpPatch("name/{id}")]
    [Authorize(Roles = "User, Shelter, Admin, SuperAdmin, Worker")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> UpdateFileName([Required] string newName, [Required] Guid id)
    {
        var user = HttpContext.User.FindFirstValue("userId");
        if (user is null)
        {
            throw new UnauthorizedException("invalid_token", "Invalid token");
        }
        
        await _blobService.UpdateFileName(id, newName ,new Guid(user));

        return NoContent();
    }

    /// <summary>
    ///     Usuwanie listy plików o wskazanych Id.
    /// </summary>
    [HttpDelete]
    [Authorize(Roles = "Shelter, SuperAdmin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> DeleteListOfFiles([Required][FromBody] List<string> filesIds)
    {
        await _blobService.DeleteListOfFiles(filesIds);

        return NoContent();
    }
}