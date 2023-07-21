using FluentValidation;
using LapkaBackend.Application.Requests;

namespace LapkaBackend.Application.Validators;

public class UserRegistrationRequestValidator : AbstractValidator<UserRegistrationRequest>
{
    public UserRegistrationRequestValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty()
            .MinimumLength(2)
            .MaximumLength(50)
            .WithErrorCode("invalid_first_name");
        RuleFor(x => x.LastName)
            .NotEmpty()
            .MinimumLength(2)
            .MaximumLength(50)
            .WithErrorCode("invalid_last_name");
        RuleFor(x => x.EmailAddress)
            .NotEmpty()
            .EmailAddress()
            .WithErrorCode("invalid_email");
        RuleFor(x => x.Password)
            .NotEmpty()
            .Matches("^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])(?=.*?[#?!@$%^&*-]).{8,}$")
            .WithErrorCode("invalid_password")
            .WithMessage("Required at least 8 characters, one non alphanumeric character, one digit, one uppercase and one lowercase.");
        RuleFor(x => x.ConfirmPassword)
            .Equal(x => x.Password)
            .WithMessage("Passwords do not match")
            .WithErrorCode("invalid_confirm_password");
    }
}