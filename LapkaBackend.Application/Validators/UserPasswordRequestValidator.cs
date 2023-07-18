using FluentValidation;
using LapkaBackend.Application.Requests;

namespace LapkaBackend.Application.Validators
{
    public class UserPasswordRequestValidator : AbstractValidator<UserPasswordRequest>
    {
        public UserPasswordRequestValidator()
        {
            RuleFor(x=> x.CurrentPassword) 
            .NotEmpty()
            .Matches("^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])(?=.*?[#?!@$%^&*-]).{8,}$")
            .WithErrorCode("invalid_password")
            .WithMessage("Required at least 8 characters, one non alphanumeric character, one digit, one uppercase and one lowercase.");

            RuleFor(x => x.NewPassword)
            .NotEmpty()
            .Matches("^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])(?=.*?[#?!@$%^&*-]).{8,}$")
            .WithErrorCode("invalid_password")
            .WithMessage("Required at least 8 characters, one non alphanumeric character, one digit, one uppercase and one lowercase.");

            RuleFor(x => x.ConfirmNewPassword)
                .Equal(x => x.NewPassword)
                .WithMessage("Passwords do not match")
                .WithErrorCode("invalid_confirm_password");
        }
    }
}
