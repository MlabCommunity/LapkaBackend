using LapkaBackend.Application.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;


namespace LapkaBackend.Application.Functions.Queries
{
    public record PetListInShelterQuery(Guid ShelterId, int PageNumber=1, int PageSize=10,string SortParam="", bool Asc=false) : IRequest<PetListInShelterResponse>;

    public class PetListInShelterQueryHandler : IRequestHandler<PetListInShelterQuery, PetListInShelterResponse>
    {
        private readonly IDataContext _dbContext;

        public PetListInShelterQueryHandler(IDataContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<PetListInShelterResponse> Handle(PetListInShelterQuery request, CancellationToken cancellationToken)
        {
            int totalItemsCount = await _dbContext.Animals.Where(x => x.ShelterId == request.ShelterId).CountAsync();
            int numberOfPages =  (int)Math.Ceiling((double)((float)totalItemsCount / (float)request.PageSize));

            var query = _dbContext.Animals
                .Include(a => a.AnimalCategory)
                .Include( a=> a.AnimalViews)
                .Where(a => a.ShelterId == request.ShelterId);

            if (!string.IsNullOrEmpty(request.SortParam))
            {
                switch (request.SortParam.ToLower())
                {
                    case "name":
                        if (request.Asc == true)
                            query = query.OrderBy(x => x.Name);
                        else
                            query = query.OrderByDescending(x => x.Name);
                        break;
                    case "species":
                        if (request.Asc == true)
                            query = query.OrderBy(x => x.Species);
                        else
                            query = query.OrderByDescending(x => x.Species);
                        break;
                    case "gender":
                        if (request.Asc == true)
                            query = query.OrderBy(x => x.Gender);
                        else
                            query = query.OrderByDescending(x => x.Gender);
                        break;
                    case "marking":
                        if (request.Asc == true)
                            query = query.OrderBy(x => x.Marking);
                        else
                            query = query.OrderByDescending(x => x.Marking);
                        break;
                    case "weight":
                        if (request.Asc == true)
                            query = query.OrderBy(x => x.Weight);
                        else
                            query = query.OrderByDescending(x => x.Weight);
                        break;
                    case "issterilized":
                        if (request.Asc == true)
                            query = query.OrderBy(x => x.IsSterilized);
                        else
                            query = query.OrderByDescending(x => x.IsSterilized);
                        break;
                    case "age":
                        if (request.Asc == true)
                            query = query.OrderBy(x => x.Months);
                        else
                            query = query.OrderByDescending(x => x.Months);
                        break;
                    case "category":
                        if (request.Asc == true)
                            query = query.OrderBy(x => x.AnimalCategory.CategoryName);
                        else
                            query = query.OrderByDescending(x => x.AnimalCategory.CategoryName);
                        break;                 
                    case "views":
                        if (request.Asc == true)
                            query = query.OrderBy(x => x.AnimalViews.Count);
                        else
                            query = query.OrderByDescending(x => x.AnimalViews.Count);
                        break;
                    case "createdat":
                        if (request.Asc == true)
                            query = query.OrderBy(x => x.CreatedAt);
                        else
                            query = query.OrderByDescending(x => x.CreatedAt);                      
                        break;
                    default:
                        query = query.OrderBy(x => x.Name);
                        break;
                }
            }

            var FoundAnimals = await query
                .Skip(request.PageSize * (request.PageNumber - 1)).Take(request.PageSize)
                .ToListAsync();

            var petsList =  FoundAnimals.Select( p => new PetInListInShelterDto()
            {
                PetId = p.Id,
                Name = p.Name,
                AnimalCategory = p.AnimalCategory.CategoryName,
                Gender = p.Gender,
                Species = p.Species,
                Marking = p.Marking,
                Weight = (float)p.Weight,
                Photos = _dbContext.Blobs
                            .Where(x => x.ParentEntityId == p.Id)
                            .Select(blob => blob.Id.ToString())
                            .ToArray(),
                Months = p.Months,
                CreatedAt = p.CreatedAt,
                IsSterilized = p.IsSterilized,
                Description = p.Description,
                Views = p.AnimalViews.Count,
                IsVisible = p.IsVisible

            })
            .ToList();

            var petListResponse = new PetListInShelterResponse()
            {
                PetInListInShelterDto = petsList,
                TotalPages = numberOfPages,
                TotalItemsCount = totalItemsCount
            };


            return petListResponse;
        }
    }



    public class PetInListInShelterDto
    {
        public Guid PetId { get; set; }
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
        public string[]? Photos { get; set; }       
        public int Views { get; set; }
    }

    public class PetListInShelterResponse
    {
        public List<PetInListInShelterDto>? PetInListInShelterDto { get; set; }
        public int TotalPages { get; set; }
        public int TotalItemsCount { get; set; }
    }

    public class PetListInShelterRequest
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string? SortParam { get; set; } = null;
        public bool Asc { get; set; } = false;
    }
}
