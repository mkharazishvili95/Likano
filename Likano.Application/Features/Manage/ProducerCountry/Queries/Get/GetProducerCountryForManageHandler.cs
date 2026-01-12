using Likano.Application.Interfaces;
using MediatR;

namespace Likano.Application.Features.Manage.ProducerCountry.Queries.Get
{
    public class GetProducerCountryForManageHandler : IRequestHandler<GetProducerCountryForManageQuery, GetProducerCountryForManageResponse>
    {
        readonly IManageRepository _repository;
        public GetProducerCountryForManageHandler(IManageRepository repository)
        {
            _repository = repository;
        }

        public async Task<GetProducerCountryForManageResponse> Handle(GetProducerCountryForManageQuery request, CancellationToken cancellationToken)
        {
            var country = await _repository.GetProducerCountry(request.Id);
            if (country is null)
            {
                return new GetProducerCountryForManageResponse
                {
                    Success = false,
                    StatusCode = 404,
                    Message = "Producer country not found"
                };
            }

            return new GetProducerCountryForManageResponse
            {
                Success = true,
                StatusCode = 200,
                Id = country.Id,
                Name = country.Name
            };
        }
    }
}