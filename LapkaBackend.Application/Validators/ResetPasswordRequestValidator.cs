using FluentValidation;
using LapkaBackend.Application.Requests;

namespace LapkaBackend.Application.Validators;

public class ResetPasswordRequestValidator : AbstractValidator<ResetPasswordRequest>
{
    public ResetPasswordRequestValidator()
    {
        RuleFor(x => x.Password)
            .NotEmpty()
            .Matches("^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])(?=.*?[#?!@$%^&*-]).{8,}$")
            .WithErrorCode("invalid_password")
            .WithMessage(
                "Required at least 8 characters, one non alphanumeric character, one digit, one uppercase and one lowercase.");

        RuleFor(x => x.ConfirmPassword)
            .Equal(x => x.Password)
            .WithMessage("Passwords do not match")
            .WithErrorCode("invalid_confirm_password");
    }
}