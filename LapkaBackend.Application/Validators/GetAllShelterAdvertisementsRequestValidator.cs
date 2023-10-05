using FluentValidation;
using LapkaBackend.Application.Requests;

namespace LapkaBackend.Application.Validators;

public class GetAllShelterAdvertisementsRequestValidator : AbstractValidator<GetAllShelterAdvertisementsRequest>
{
    public GetAllShelterAdvertisementsRequestValidator()
    {
        
    }
}