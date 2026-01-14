using Likano.Application.Interfaces;
using MediatR;

namespace Likano.Application.Features.Manage.ProducerCountry.Commands.Change
{
    public class ChangeCountryHandler : IRequestHandler<ChangeCountryCommand, ChangeCountryResponse>
    {
        readonly IManageRepository _repository;
        public ChangeCountryHandler(IManageRepository repository)
        {
            _repository = repository;
        }

        public async Task<ChangeCountryResponse> Handle(ChangeCountryCommand request, CancellationToken cancellationToken)
        {
            var product = await _repository.GetProduct(request.ProductId);
            if (product == null)
                return new ChangeCountryResponse { Message = "Product not found.", Success = false, StatusCode = 404 };

            var newCountry = await _repository.GetProducerCountry(request.NewCountryId);
            if (newCountry == null)
                return new ChangeCountryResponse { Message = "Country not found.", Success = false, StatusCode = 404 };

            product.ProducerCountryId = request.NewCountryId;
            await _repository.ChangeCountry(request.ProductId, request.NewCountryId);

            return new ChangeCountryResponse { Message = "ქვეყანა წარმატებით შეიცვალა", Success = true, StatusCode = 200 };
        }
    }
}
