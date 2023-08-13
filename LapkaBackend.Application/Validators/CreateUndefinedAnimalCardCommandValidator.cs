using FluentValidation;
using LapkaBackend.Application.Functions.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LapkaBackend.Application.Validators;

    public class CreateUndefinedAnimalCardCommandValidator : AbstractValidator<CreateUndefinedAnimalCardCommand>
    {
        public CreateUndefinedAnimalCardCommandValidator()
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
                    .Must(m => m > 0)
                    .WithMessage("Invalid Value. Number of nonths must be greater than 0")
                    .WithErrorCode("invalid_number_of_months");

            RuleFor(x => x.Weight)
                    .NotEmpty()
                    .Must(m => m > 0)
                    .WithMessage("Invalid Value. Weight must be greater than 0")
                    .WithErrorCode("invalid_weight");

            RuleFor(x => x.ShelterId)
                    .NotEmpty()
                    .MaximumLength(32)
                    .WithMessage("Invalid Description. Description must have less then 150 chars")
                    .WithErrorCode("invalid_shelterId");

            RuleFor(x => x.Photos)
                    .Must(photos => photos.Length <= 5)
                    .WithMessage("Photos array can have a maximum of 5 elements.")
                    .WithErrorCode("invalid_number_of_colors");
        }
    }

