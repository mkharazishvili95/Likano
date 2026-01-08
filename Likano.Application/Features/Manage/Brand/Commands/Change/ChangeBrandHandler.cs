using Likano.Application.Interfaces;
using MediatR;

namespace Likano.Application.Features.Manage.Brand.Commands.Change
{
    public class ChangeBrandHandler : IRequestHandler<ChangeBrandCommand, ChangeBrandResponse>
    {
        readonly IManageRepository _repository;
        public ChangeBrandHandler(IManageRepository repository)
        {
            _repository = repository;
        }

        public async Task<ChangeBrandResponse> Handle(ChangeBrandCommand request, CancellationToken cancellationToken)
        {
            var product = await _repository.GetProduct(request.ProductId);
            if (product == null)
                return new ChangeBrandResponse { Message = "Product not found.", Success = false, StatusCode = 404 };

            var brand = await _repository.GetBrand(request.NewBrandId);

            if (brand == null || !brand.IsActive)
                return new ChangeBrandResponse { Message = "Brand not found or its not active.", Success = false, StatusCode = 404 };

            if (product.BrandId == request.NewBrandId)
                return new ChangeBrandResponse { Message = "Product already in this Brand.", Success = false, StatusCode = 400 };

            product.BrandId = request.NewBrandId;
            await _repository.ChangeBrand(request.ProductId, request.NewBrandId);

            return new ChangeBrandResponse { Message = "ბრენდი წარმატებით შეიცვალა", Success = true, StatusCode = 200 };
        }
    }
}
