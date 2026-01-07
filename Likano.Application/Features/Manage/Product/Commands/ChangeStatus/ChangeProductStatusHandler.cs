using Likano.Application.Interfaces;
using MediatR;

namespace Likano.Application.Features.Manage.Product.Commands.ChangeStatus
{
    public class ChangeProductStatusHandler : IRequestHandler<ChangeProductStatusCommand, ChangeProductStatusResponse>
    {
        readonly IManageRepository _manageRepository;
        public ChangeProductStatusHandler(IManageRepository manageRepository)
        {
            _manageRepository = manageRepository;
        }

        public async Task<ChangeProductStatusResponse> Handle(ChangeProductStatusCommand request, CancellationToken cancellationToken)
        {
            var product = await _manageRepository.GetProduct(request.ProductId);

            if(product == null)
                return new ChangeProductStatusResponse { Message = "Product not found.", Success = false, StatusCode = 404 };

            if(product.Status == request.Status)
                return new ChangeProductStatusResponse { Message = "Product already in the desired status.", Success = false, StatusCode = 400 };

            var result = await _manageRepository.ChangeStatus(request.ProductId, request.Status);
            if (!result)
                return new ChangeProductStatusResponse { Message = "სტატუსის შეცვლისას მოხდა შეცდომა", Success = false, StatusCode = 500 };
            return new ChangeProductStatusResponse { Message = "სტატუსი წარმატებით შეიცვალა", Success = true, StatusCode = 200 };
        }
    }
}
