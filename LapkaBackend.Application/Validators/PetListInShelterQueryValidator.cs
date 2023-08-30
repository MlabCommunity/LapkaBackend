using FluentValidation;
using LapkaBackend.Application.Functions.Command;
using LapkaBackend.Application.Functions.Queries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LapkaBackend.Application.Validators
{
    public class PetListInShelterQueryValidator : AbstractValidator<PetListInShelterRequest>
    {
        public PetListInShelterQueryValidator()
        {
            RuleFor(x => x.SortParam)
                .MinimumLength(2)
                .MaximumLength(64)
                .WithErrorCode("invalid_sort_parametr");

        }
    }
}
