using LapkaBackend.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

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
    ///     Dodawanie pliku 1GB i zwrócenie jego identyfikatora. Dostępne dla schroniska.
    /// </summary>
    [HttpPost]
    //[Authorize(Roles = "Shelter")]
    [RequestSizeLimit(1073741824)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> SaveFileByShelter(IFormFile file)
    {
        return Ok(await _blobService.UploadFileAsShelterAsync(file, new Guid("0B9B21CE-717C-4F3A-A977-08DB8E7E7965")));
    }

    /// <summary>
    ///     Dodawanie zdjęcia do 5MB (.jpg, .jpeg, .bmp, .png) i zwrócenie jego identyfikatora. Dostępne dla zalogowanego użytkownika.
    /// </summary>
    [HttpPost("picture")]
    [Authorize(Roles = "User")]
    [RequestSizeLimit(5242880)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> SaveFileByUser(IFormFile file)
    {
        return Ok(await _blobService.UploadFileAsUserAsync(file, new Guid(HttpContext.User.FindFirstValue("userId")!)));
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
    ///     Zastąpienie pliku o wskazanym Id nowym plikiem do 1GB 
    /// </summary>
    [HttpPut("{id}")]
    [RequestSizeLimit(1073741824)]
    //[Authorize(Roles = "Shelter")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> UpdateFileShelter([Required]IFormFile file, [Required]Guid id)
    {
        await _blobService.UpdateFileAsShelterAsync(file, id);

        return NoContent();
    }

    /// <summary>
    ///     Zastąpienie zdjęcia o wskazanym Id nowym plikiem do 5MB 
    /// </summary>
    [HttpPut("picture/{id}")]
    [RequestSizeLimit(5242880)]
    //[Authorize(Roles = "User")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> UpdateFileUser([Required] IFormFile file, [Required] Guid id)
    {
        await _blobService.UpdateFileAsUserAsync(file, id);

        return NoContent();
    }
    
    /// <summary>
    ///     Edytowanie nazwy plikuu o wskazanym Id 
    /// </summary>
    [HttpPatch("name/{id}")]
    [Authorize(Roles = "User, Shelter")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> UpdateFileName([Required] string newName, [Required] Guid id)
    {
        await _blobService.UpdateFileName(id, newName ,new Guid(HttpContext.User.FindFirstValue("userId")!));

        return NoContent();
    }

    /// <summary>
    ///     Usuwanie listy plików o wskazanych Id.
    /// </summary>
    [HttpDelete]
    [Authorize(Roles = "Shelter")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> DeleteListOfFiles([Required][FromBody] List<string> filesIds)
    {
        await _blobService.DeleteListOfFiles(filesIds);

        return NoContent();
    }
}