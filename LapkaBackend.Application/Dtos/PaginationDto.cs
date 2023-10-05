using System.ComponentModel;

namespace LapkaBackend.Application.Dtos;

public class PaginationDto
{
    [DefaultValue(1)]
    public int PageNumber { get; set; } = 1;
    [DefaultValue(10)]
    public int PageSize { get; set; } = 10;
    public bool AscendingSort { get; set; } = true;
    // true - ascending, false - descending
    public string? SearchText { get; set; } = "";
}