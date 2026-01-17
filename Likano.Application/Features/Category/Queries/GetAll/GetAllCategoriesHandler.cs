using Likano.Application.Interfaces;
using Likano.Domain.Entities;
using MediatR;

namespace Likano.Application.Features.Category.Queries.GetAll
{
    public class GetAllCategoriesHandler : IRequestHandler<GetAllCategoriesQuery, GetAllCategoriesResponse>
    {
        readonly ICategoryRepository _repository;

        public GetAllCategoriesHandler(ICategoryRepository repository)
        {
            _repository = repository;
        }

        public async Task<GetAllCategoriesResponse> Handle(GetAllCategoriesQuery request, CancellationToken cancellationToken)
        {
            var pageNumber = request.Pagination?.PageNumber > 0 ? request.Pagination.PageNumber : 1;
            var pageSize = request.Pagination?.PageSize > 0 ? request.Pagination.PageSize : 20;
            var search = request.SearchString?.Trim();

            var categories = await _repository.GetAll() ?? new List<Domain.Entities.Category>();

            if (!string.IsNullOrWhiteSpace(search))
            {
                categories = categories
                    .Where(c =>
                        (!string.IsNullOrEmpty(c.Name) && c.Name.Contains(search, StringComparison.OrdinalIgnoreCase)) ||
                        (!string.IsNullOrEmpty(c.Description) && c.Description.Contains(search, StringComparison.OrdinalIgnoreCase)))
                    .ToList();
            }

            if (request.Id != null)
            {
                categories = categories
                    .Where(c => c.Id == request.Id)
                    .ToList();
            }

            var totalCount = categories.Count;

            var paged = categories
                .OrderBy(c => c.Id)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return new GetAllCategoriesResponse
            {
                Success = true,
                StatusCode = 200,
                TotalCount = totalCount,
                Items = paged.Select(c => new GetAllCategoriesItemsResponse
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