using Likano.Application.Interfaces;
using MediatR;

namespace Likano.Application.Features.Manage.Product.Commands.ChangeType
{
    public class ChangeTypeHandler : IRequestHandler<ChangeTypeCommand, ChangeTypeResponse>
    {
        readonly IManageRepository _repository;
        public ChangeTypeHandler(IManageRepository repository)
        {
            _repository = repository;
        }

        public async Task<ChangeTypeResponse> Handle(ChangeTypeCommand request, CancellationToken cancellationToken)
        {
            if(request.ProductId <= 0 || request.NewType == null || request.NewType <= 0)
                return new ChangeTypeResponse { Message = "request is required.", Success = false, StatusCode = 400 };

            var product = await _repository.GetProduct(request.ProductId);  

            if(product == null)
                return new ChangeTypeResponse { Message = "Product not found.", Success = false, StatusCode = 404 };

            var result = await _repository.ChangeProductType(request.ProductId, request.NewType);

            if(!result)
                return new ChangeTypeResponse { Message = "Failed to change product type.", Success = false, StatusCode = 500 };

            return new ChangeTypeResponse { Message = "პროდუქტის ტიპი წარმატებით შეიცვალა", Success = true, StatusCode = 200 };
        }
    }
}
