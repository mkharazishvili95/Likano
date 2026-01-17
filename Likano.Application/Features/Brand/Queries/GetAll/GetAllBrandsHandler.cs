using Likano.Application.Interfaces;
using MediatR;

namespace Likano.Application.Features.Brand.Queries.GetAll
{
    public class GetAllBrandsHandler : IRequestHandler<GetAllBrandsQuery, GetAllBrandsResponse>
    {
        readonly IBrandRepository _repository;

        public GetAllBrandsHandler(IBrandRepository repository)
        {
            _repository = repository;
        }

        public async Task<GetAllBrandsResponse> Handle(GetAllBrandsQuery request, CancellationToken cancellationToken)
        {
            var pageNumber = request.Pagination?.PageNumber > 0 ? request.Pagination.PageNumber : 1;
            var pageSize = request.Pagination?.PageSize > 0 ? request.Pagination.PageSize : 20;
            var search = request.SearchString?.Trim();

            var brands = await _repository.GetAll() ?? new List<Domain.Entities.Brand>();

            if (!string.IsNullOrWhiteSpace(search))
            {
                brands = brands
                    .Where(c =>
                        (!string.IsNullOrEmpty(c.Name) && c.Name.Contains(search, StringComparison.OrdinalIgnoreCase)) ||
                        (!string.IsNullOrEmpty(c.Description) && c.Description.Contains(search, StringComparison.OrdinalIgnoreCase)))
                    .ToList();
            }

            var totalCount = brands.Count;

            var paged = brands
                .OrderBy(c => c.Id)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return new GetAllBrandsResponse
            {
                Success = true,
                StatusCode = 200,
                TotalCount = totalCount,
                Items = paged.Select(c => new GetAllBrandsItemsResponse
                {
                    Id = c.Id,
                    Name = c.Name,
                    Description = c.Description,
                    Logo = c.Logo
                }).ToList()
            };
        }
    }
}
