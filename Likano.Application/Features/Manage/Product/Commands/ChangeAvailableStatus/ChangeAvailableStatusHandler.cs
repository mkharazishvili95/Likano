using Likano.Application.Interfaces;
using MediatR;

namespace Likano.Application.Features.Manage.Product.Commands.ChangeAvailableStatus
{
    public class ChangeAvailableStatusHandler : IRequestHandler<ChangeAvailableStatusCommand, ChangeAvailableStatusResponse>
    {
        readonly IManageRepository _repository;
        public ChangeAvailableStatusHandler(IManageRepository repository)
        {
            _repository = repository;
        }

        public async Task<ChangeAvailableStatusResponse> Handle(ChangeAvailableStatusCommand request, CancellationToken cancellationToken)
        {
            var product = await _repository.GetProduct(request.ProductId);
            if (product == null)
                return new ChangeAvailableStatusResponse { Message = "პროდუქტი ვერ მოიძებნა.", Success = false, StatusCode = 404 };

            product.IsAvailable = request.IsAvailable;
            await _repository.ChangeProductAvailableStatus(product.Id, request.IsAvailable);
            return new ChangeAvailableStatusResponse { Message = "პროდუქტის ხელმისაწვდომობის სტატუსი წარმატებით შეიცვალა.", Success = true, StatusCode = 200 };
        }
    }
}
