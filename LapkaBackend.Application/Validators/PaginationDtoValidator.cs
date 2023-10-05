using FluentValidation;
using LapkaBackend.Application.Dtos;

namespace LapkaBackend.Application.Validators;

public class PaginationDtoValidator : AbstractValidator<PaginationDto>
{
    public PaginationDtoValidator()
    {
        RuleFor(x => x.PageNumber).GreaterThan(0)
            .WithErrorCode("invalid_page_number").WithMessage("Page number must be greater than 0");
        
        RuleFor(x => x.PageSize).GreaterThan(0)
            .WithErrorCode("invalid_page_size").WithMessage("Page size must be greater than 0");
        
        RuleFor(x => x.SearchText).Must(y => y != null && !y.StartsWith(" "))
            .WithErrorCode("invalid_search_text").WithMessage("Search text cannot start with space");
    }
}