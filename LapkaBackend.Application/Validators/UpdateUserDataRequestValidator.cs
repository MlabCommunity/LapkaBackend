using FluentValidation;
using LapkaBackend.Application.Requests;

namespace LapkaBackend.Application.Validators;

public class UpdateUserDataRequestValidator : AbstractValidator<UpdateUserDataRequest>
{
    public UpdateUserDataRequestValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty()
            .WithErrorCode("invalid_first_name");
        RuleFor(x => x.LastName)
            .NotEmpty()
            .WithErrorCode("invalid_last_name");
    }
}