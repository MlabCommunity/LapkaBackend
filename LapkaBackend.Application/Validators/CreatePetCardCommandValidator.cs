using FluentValidation;
using LapkaBackend.Application.Functions.Command;


namespace LapkaBackend.Application.Validators;

    public class CreatePetCardCommandValidator : AbstractValidator<CreatePetCardCommand>
    {
        public CreatePetCardCommandValidator()
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
                .WithMessage("Invalid Description. Description must be up to 150 characters")
                .WithErrorCode("invalid_description");

        RuleFor(x => x.Months)
                .NotEmpty()
                .WithMessage("Invalid Value. Number of nonths must be greater than 0")
                .WithErrorCode("invalid_number_of_months");

        RuleFor(x => x.Weight)
                .Must(m => m > 0)
                .WithMessage("Invalid Value. Weight must be greater than 0")
                .WithErrorCode("invalid_Weight");

        RuleFor(x => x.Color)
                .NotEmpty()
                .MaximumLength(50)
                .WithMessage("Invalid Description. Description must have less then 150 chars")
                .WithErrorCode("invalid_color");

        RuleFor(x => x.AnimalCategory)
                .NotEmpty()
                .IsInEnum()
                .WithMessage("Invalid animal category")
                .WithErrorCode("invalid_animal_category");

        RuleFor(x => x.Breed)
                .NotEmpty()
                .MaximumLength(50)
                .WithMessage("Invalid animal category")
                .WithErrorCode("invalid_breed");

        RuleFor(x => x.Photos)
                .Must(photos => photos.Count <= 5)
                .WithMessage("Photos array can have a maximum of 5 elements.")
                .WithErrorCode("invalid_number_of_photos");

    }
    }

