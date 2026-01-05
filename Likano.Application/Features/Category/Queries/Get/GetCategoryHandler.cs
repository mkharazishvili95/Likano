using Likano.Application.Interfaces;
using MediatR;

namespace Likano.Application.Features.Category.Queries.Get
{
    public class GetCategoryHandler : IRequestHandler<GetCategoryQuery, GetCategoryResponse>
    {
        readonly ICategoryRepository _repository;
        public GetCategoryHandler(ICategoryRepository repository)
        {
            _repository = repository;
        }

        public async Task<GetCategoryResponse> Handle(GetCategoryQuery request, CancellationToken cancellationToken)
        {
            if(request == null)
                return new GetCategoryResponse { Description = "request required.", Success = false, StatusCode = 400 };

            var category = await _repository.Get(request.Id);

            if (category == null)
                return new GetCategoryResponse { Description = "Category not found.", Success = false, StatusCode = 404 };

            return new GetCategoryResponse 
            { 
                Id = category.Id, 
                Name = category.Name, 
                Description = category.Description, 
                Logo = category.Logo, 
                Success = true, 
                StatusCode = 200 
            };
        }
    }
}
