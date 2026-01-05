using Likano.Application.Interfaces;
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
            var categories = await _repository.GetAll();

            if (categories == null || categories.Count == 0)
            {
                return new GetAllCategoriesResponse
                {
                    Success = true,
                    StatusCode = 200,
                    Message = "No categories found.",
                    Items = new List<GetAllCategoriesItemsResponse>(),
                    TotalCount = 0
                };
            }

            var totalCount = categories.Count;

            return new GetAllCategoriesResponse
            {
                Success = true,
                StatusCode = 200,
                TotalCount = totalCount,
                Items = categories.Select(c => new GetAllCategoriesItemsResponse
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