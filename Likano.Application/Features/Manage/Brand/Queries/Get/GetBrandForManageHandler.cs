using Likano.Application.Interfaces;
using MediatR;

namespace Likano.Application.Features.Manage.Brand.Queries.Get
{
    public class GetBrandForManageHandler : IRequestHandler<GetBrandForManageQuery, GetBrandForManageResponse>
    {
        readonly IManageRepository _repository;
        public GetBrandForManageHandler(IManageRepository repository)
        {
            _repository = repository;
        }

        public async Task<GetBrandForManageResponse> Handle(GetBrandForManageQuery request, CancellationToken cancellationToken)
        {
            var brand = await _repository.GetBrand(request.BrandId);
            if (brand == null)
                return new GetBrandForManageResponse { Description = "Brand not found", Success = false, StatusCode = 404 };

            return new GetBrandForManageResponse
            {
                Id = brand.Id,
                Name = brand.Name,
                Description = brand.Description,
                Logo = brand.Logo,
                IsActive = brand.IsActive,
                Success = true,
                StatusCode = 200
            };
        }
    }
}
