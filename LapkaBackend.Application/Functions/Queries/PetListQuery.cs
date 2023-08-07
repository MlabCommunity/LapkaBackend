using AutoMapper;
using LapkaBackend.Application.Common;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace LapkaBackend.Application.Functions.Queries
{
    public record PetListQuery(int PageNumber, int PageSize) : IRequest<List<PetInListDto>>;

    public class PetListQueryHandler : IRequestHandler<PetListQuery, List<PetInListDto>>
    {
        private readonly IDataContext _dbContext;
        private readonly IMapper _mapper;

        public PetListQueryHandler(IDataContext dbContext, IMapper mapper)
        {

            _dbContext = dbContext;
            _mapper = mapper;
        }

        public async Task<List<PetInListDto>> Handle(PetListQuery request, CancellationToken cancellationToken)
        {
            int totalItemsCount = _dbContext.Animals.Count();
            int numberOfPages =  (int)Math.Ceiling((double)((float)totalItemsCount / (float)request.PageSize));

            var FoundAnimals =  _dbContext.Animals
                .Include(a => a.Photos).Include(a => a.AnimalCategory)
                .Where(a => a.IsVisible)
                .OrderBy(x => x.Name)
                .Skip(request.PageSize * (request.PageNumber-1)).Take(request.PageSize)
                .ToList();

            var PetsList = FoundAnimals.Select(p => new PetInListDto()
            {
                Id = p.Id,
                Name = p.Name,
                Type = p.AnimalCategory.CategoryName,
                Gender = p.Gender,
                Breed = p.Species,
                Color = p.Marking,
                Weight = (float)p.Weight,
                //ProfilePhoto = p.Photos.FirstOrDefault(p => p.IsProfilePhoto = true),
                //Photos = p.Photos.FindAll(p => p.IsProfilePhoto != true)
                Months = p.Months,
                CreatedAt = p.CreatedAt,
                IsSterilized = p.IsSterilized,
                Description = p.Description,
                

            })
            .ToList();

            var petListResponse = new PetListResponse()
            {
                PetInListDto = PetsList,
                TotalPages = numberOfPages,
                TotalItemsCount = totalItemsCount
            };


            return PetsList.ToList();
        }
    }

    public class PetInListDto
    {
        public Guid Id { get; set; } 
        public string Name { get; set; } = null!;
        public string Type { get; set; } = null!;
        public string Gender { get; set; } = null!;
        public string Breed { get; set; } = null!;
        public string Color { get; set; } = null!;
        public float Weight { get; set; }
        public string ProfilePhoto { get; set; } = null!;
        public string Photos { get; set; } = null!;
        public int Months { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsSterilized { get; set; }
        public string Description { get; set; } = null!;
    }

    public class PetListResponse
    {
        public List<PetInListDto>? PetInListDto { get; set; }
        public int TotalPages { get; set; }
        public int TotalItemsCount { get; set; }
    }
}
