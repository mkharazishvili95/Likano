using Likano.Application.Interfaces;
using MediatR;

namespace Likano.Application.Features.Manage.ProducerCountry.Commands.Create
{
    public class CreateProducerCountryForManageHandler : IRequestHandler<CreateProducerCountryForManageCommand, CreateProducerCountryForManageResponse>
    {
        readonly IManageRepository _repository;
        public CreateProducerCountryForManageHandler(IManageRepository repository)
        {
            _repository = repository;
        }

        public async Task<CreateProducerCountryForManageResponse> Handle(CreateProducerCountryForManageCommand request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.Name))
            {
                return new CreateProducerCountryForManageResponse
                {
                    Success = false,
                    StatusCode = 400,
                    Message = "Name is required"
                };
            }

            var country = new Likano.Domain.Entities.ProducerCountry { Name = request.Name.Trim() };
            var result = await _repository.AddProducerCountry(country);
            if (!result)
            {
                return new CreateProducerCountryForManageResponse
                {
                    Success = false,
                    StatusCode = 500,
                    Message = "Failed to add producer country"
                };
            }

            return new CreateProducerCountryForManageResponse
            {
                Success = true,
                StatusCode = 201,
                Id = country.Id,
                Message = "მწარმოებელი ქვეყანა წარმატებით შეიქმნა"
            };
        }
    }
}