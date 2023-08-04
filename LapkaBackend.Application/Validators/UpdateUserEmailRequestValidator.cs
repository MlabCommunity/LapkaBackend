using FluentValidation;
using LapkaBackend.Application.Requests;

namespace LapkaBackend.Application.Validators
{
    public class UpdateUserEmailRequestValidator : AbstractValidator<UpdateUserEmailRequest>
    {
        public UpdateUserEmailRequestValidator()
        {
            RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress()
            .WithErrorCode("invalid_email");
        }
    }
}
