using AutoMapper;
using LapkaBackend.Application.Common;
using LapkaBackend.Application.Interfaces;
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
    public record PetListQuery(int PageNumber=1, int PageSize=10) : IRequest<PetListResponse>;

    public class PetListQueryHandler : IRequestHandler<PetListQuery, PetListResponse>
    {
        private readonly IDataContext _dbContext;
        private readonly IBlobService _blobService;

        public PetListQueryHandler(IDataContext dbContext, IBlobService blobService)
        {
            _dbContext = dbContext;
            _blobService = blobService;
        }

        public async Task<PetListResponse> Handle(PetListQuery request, CancellationToken cancellationToken)
        {
            int totalItemsCount = _dbContext.Animals.Count();
            int numberOfPages =  (int)Math.Ceiling((double)((float)totalItemsCount / (float)request.PageSize));

            var FoundAnimals = await  _dbContext.Animals
                .Include(a => a.AnimalCategory)
                .Include(a=> a.AnimalViews)
                .OrderBy(x => x.Name)
                .Skip(request.PageSize * (request.PageNumber-1)).Take(request.PageSize)
                .ToListAsync();

            var petsList = FoundAnimals.Select(p => new PetInListDto()
            {
                Id = p.Id,
                Name = p.Name,
                AnimalCategory = p.AnimalCategory.CategoryName,
                Gender = p.Gender,
                Species = p.Species,
                Marking = p.Marking,
                Weight = (float)p.Weight,
                Photos = _dbContext.Blobs
                            .Where(x => x.ParentEntityId == p.Id && x.Id.ToString() != p.ProfilePhoto)
                            .Select(blob => blob.Id.ToString())
                            .ToArray(),
                Months = p.Months,
                CreatedAt = p.CreatedAt,
                IsSterilized = p.IsSterilized,
                Description = p.Description,
                IsVisible = p.IsVisible,
                Views = p.AnimalViews.Count

            })
            .ToList();

            var petListResponse = new PetListResponse()
            {
                PetInListDto = petsList,
                TotalPages = numberOfPages,
                TotalItemsCount = totalItemsCount
            };


            return petListResponse;
        }
    }

    public class PetInListDto
    {
        public Guid Id { get; set; } 
        public string Name { get; set; } = null!;
        public string Species { get; set; } = null!;
        public string Gender { get; set; } = null!;
        public string Marking { get; set; } = null!;
        public float Weight { get; set; }
        public string Description { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
        public bool IsSterilized { get; set; }
        public bool IsVisible { get; set; }
        public int Months { get; set; }
        public string AnimalCategory { get; set; } = null!;      
        public string[]? Photos { get; set; } = null!;
        public int Views { get; set; }




    }

    public class PetListResponse
    {
        public List<PetInListDto>? PetInListDto { get; set; }
        public int TotalPages { get; set; }
        public int TotalItemsCount { get; set; }
    }
}
