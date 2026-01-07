using Likano.Application.Interfaces;
using MediatR;

namespace Likano.Application.Features.Manage.Category.Queries.Get
{
    public class GetCategoryForManageHandler : IRequestHandler<GetCategoryForManageQuery, GetCategoryForManageResponse>
    {
        readonly IManageRepository _repository;
        public GetCategoryForManageHandler(IManageRepository repository)
        {
            _repository = repository;
        }

        public async Task<GetCategoryForManageResponse> Handle(GetCategoryForManageQuery request, CancellationToken cancellationToken)
        {
            var category = await _repository.GetCategory(request.CategoryId);
            if(category == null)
                return new GetCategoryForManageResponse { Description = "Category not found", Success = false, StatusCode = 404 };

            return new GetCategoryForManageResponse
            {
                Id = category.Id,
                Name = category.Name,
                Description = category.Description,
                Logo = category.Logo,
                IsActive = category.IsActive, 
                Success = true,
                StatusCode = 200
            };
        }
    }
}
