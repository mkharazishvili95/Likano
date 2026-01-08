using Likano.Application.Interfaces;
using MediatR;
using System.Linq.Expressions;

namespace Likano.Application.Features.Manage.Brand.Queries.GetAll
{
    public class GetAllBrandsForManageHandler : IRequestHandler<GetAllBrandsForManageQuery, GetAllBrandsForManageResponse>
    {
        readonly IManageRepository _repository;
        public GetAllBrandsForManageHandler(IManageRepository repository)
        {
            _repository = repository;
        }

        public async Task<GetAllBrandsForManageResponse> Handle(GetAllBrandsForManageQuery request, CancellationToken cancellationToken)
        {
            var pageNumber = request.Pagination?.PageNumber > 0 ? request.Pagination.PageNumber : 1;
            var pageSize = request.Pagination?.PageSize > 0 ? request.Pagination.PageSize : 20;

            var brands = await _repository.GetAllBrands() ?? new List<Domain.Entities.Brand>();

            Expression<Func<Likano.Domain.Entities.Brand, bool>> predicate = GetAllBrandsForManageHelper.BuildWhereClause(request);
            var filtered = brands.AsQueryable().Where(predicate).ToList();

            var totalCount = filtered.Count;

            var paged = filtered
                .OrderByDescending(p => p.IsActive)
                .ThenByDescending(p => p.Id)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return new GetAllBrandsForManageResponse
            {
                Success = true,
                StatusCode = 200,
                TotalCount = totalCount,
                Items = paged.Select(p => new GetAllBrandsForManageItemsResponse
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    Logo = p.Logo,
                    IsActive = p.IsActive
                }).ToList()
            };
        }
    }
}
