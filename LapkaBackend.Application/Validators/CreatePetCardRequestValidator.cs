using FluentValidation;
using LapkaBackend.Application.Functions.Command;
using LapkaBackend.Application.Requests;


namespace LapkaBackend.Application.Validators;

    public class CreatePetCardRequestValidator : AbstractValidator<CreatePetCardRequest>
    {
        public CreatePetCardRequestValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .MaximumLength(50)
                .WithMessage("Invalid Name")
                .WithErrorCode("invalid_name");

            RuleFor(x => x.Gender)
                .IsInEnum()
                .WithMessage("Invalid gender")
                .WithErrorCode("invalid_gender");

            RuleFor(x => x.Description)
                .NotEmpty()
                .MaximumLength(200)
                .WithMessage("Invalid Description. Description must be up to 200 characters")
                .WithErrorCode("invalid_description");

            RuleFor(x => x.Months)
                .Must(m => m > 0)
                .WithMessage("Invalid Value. Number of months must be greater than 0")
                .WithErrorCode("invalid_number_of_months");

            RuleFor(x => x.Weight)
                .Must(m => m > 0)
                .WithMessage("Invalid Value. Weight must be greater than 0")
                .WithErrorCode("invalid_weight");

            RuleFor(x => x.AnimalCategory)
                .NotEmpty()
                .IsInEnum()
                .WithMessage("Invalid animal category")
                .WithErrorCode("invalid_animal_category");

            RuleFor(x => x.Species)
                .MaximumLength(50)
                .WithMessage("Invalid animal Species")
                .WithErrorCode("invalid_species");

            RuleFor(x => x.Photos)
                .Must(photos => photos.Count <= 5)
                .WithMessage("Photos array can have a maximum of 5 elements.")
                .WithErrorCode("invalid_number_of_photos");
            RuleForEach(x => x.Photos)
                .MaximumLength(50)
                .WithMessage("Maximum length of a photo filename is 50 characters.")
                .WithErrorCode("invalid_photo_length");

        }
    }

