﻿using FluentValidation;
using LapkaBackend.Application.Functions.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LapkaBackend.Application.Validators;

    public class UpdatePetCommandValidator : AbstractValidator<UpdatePetCommand>
    {
        public UpdatePetCommandValidator()
        {
            RuleFor(x => x.PetId)
                .NotEmpty()
                .WithMessage("Invalid pet Id")
                .WithErrorCode("invalid_petId");

            RuleFor(x => x.Name)
                    .NotEmpty()
                    .MaximumLength(50)
                .WithMessage("Invalid Name. Name must be up to 200 characters")
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
                    .NotEmpty()
                    .WithMessage("Invalid Value. Number of nonths must be greater than 0")
                    .WithErrorCode("invalid_number_of_months");

            RuleFor(x => x.Weight)
                    .Must(m => m > 0)
                    .WithMessage("Invalid Weight. Weight must be greater than 0")
                    .WithErrorCode("invalid_weight");

            RuleFor(x => x.Marking)
                    .NotEmpty()
                    .MaximumLength(50)
                    .WithMessage("Invalid marking. Marking must be up to 50 characters")
                    .WithErrorCode("invalid_marking");

            RuleFor(x => x.AnimalCategory)
                    .NotEmpty()
                    .IsInEnum()
                    .WithMessage("Invalid animal category")
                    .WithErrorCode("invalid_animal_category");

            RuleFor(x => x.Species)
                    .MaximumLength(50)
                    .WithMessage("Invalid Species. Species must be up to 50 characters")
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

