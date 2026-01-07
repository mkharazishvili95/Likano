using Likano.Application.Interfaces;
using MediatR;
using System.Linq.Expressions;

namespace Likano.Application.Features.Manage.Category.Queries.GetAll
{
    public class GetAllCategoriesForManageHandler : IRequestHandler<GetAllCategoriesForManageQuery, GetAllCategoriesForManageResponse>
    {
        readonly IManageRepository _repository;
        public GetAllCategoriesForManageHandler(IManageRepository repository)
        {
            _repository = repository;
        }

        public async Task<GetAllCategoriesForManageResponse> Handle(GetAllCategoriesForManageQuery request, CancellationToken cancellationToken)
        {
            var pageNumber = request.Pagination?.PageNumber > 0 ? request.Pagination.PageNumber : 1;
            var pageSize = request.Pagination?.PageSize > 0 ? request.Pagination.PageSize : 20;

            var categories = await _repository.GetAllCategories() ?? new List<Domain.Entities.Category>();

            Expression<Func<Likano.Domain.Entities.Category, bool>> predicate = GetAllCategoriesForManageHelper.BuildWhereClause(request);
            var filtered = categories.AsQueryable().Where(predicate).ToList();

            var totalCount = filtered.Count;

            var paged = filtered
                .OrderByDescending(p => p.IsActive)
                .ThenByDescending(p => p.Id)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return new GetAllCategoriesForManageResponse
            {
                Success = true,
                StatusCode = 200,
                TotalCount = totalCount,
                Items = paged.Select(p => new GetAllCategoriesForManageItemsResponse
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
