using AutoMapper;
using LapkaBackend.Application.Common;
using LapkaBackend.Application.Exceptions;
using LapkaBackend.Domain.Entities;
using LapkaBackend.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LapkaBackend.Application.Functions.Command
{
    public record UpdatePetCommand:IRequest
    {
        public Guid PetId { get; set; }
        public string Description { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string ProfilePhoto { get; set; } = null!;
        public bool IsSterilized { get; set; }
        public decimal Weight { get; set; }
        public int Months { get; set; }
        public string Gender { get; set; } = null!;
        public string[] Photos { get; set; } = null!;
        public bool IsVisible { get; set; }
        public string AnimalCategory { get; set; } = null!;
        public string Speciec { get; set; } = null!;        
        public string Marking { get; set; } = null!;
        
    }

    public class UpdatePetCommandHandler : IRequestHandler<UpdatePetCommand>
    {
        private readonly IDataContext _dbContext;
        private readonly IMapper _mapper;

        public UpdatePetCommandHandler(IDataContext dbContext, IMapper mapper)
        {

            _dbContext = dbContext;
            _mapper = mapper;
        }

        public async Task Handle(UpdatePetCommand request, CancellationToken cancellationToken)
        {
            var result = await _dbContext.Animals.FirstOrDefaultAsync(x => x.Id == request.PetId);

            if (result is null)
            {
                throw new BadRequestException("invalid_Pet", "Pet doesn't exists");
            }

            var photosList = new List<Photo>();
            for (int i = 0; i < request.Photos.Length; i++)
            {
                photosList.Add(new Photo());//dodać zapisywanie zdjęć
            }
            photosList.Add(new Photo() { IsProfilePhoto = true });//dodać zapisywanie zdjęć

            var animalCategory = await _dbContext.AnimalCategories.FirstOrDefaultAsync(r => r.CategoryName == request.AnimalCategory.ToString());
            if (animalCategory is null)
            {
                throw new BadRequestException("invalid_AnimalCategory", "Animal category doesn't exists");
            }

            result.AnimalCategory = animalCategory;
            result.Description = request.Description;
            result.Gender = request.Gender;
            result.IsSterilized = request.IsSterilized;
            result.IsVisible = request.IsVisible;
            result.Marking = request.Marking;
            result.Months = request.Months;
            result.Name = request.Name;
            result.Photos = photosList;
            result.Species = request.Speciec;
            result.Weight = request.Weight;

            _dbContext.Animals.Update(result);
            await _dbContext.SaveChangesAsync();
        }
    }
        




}
