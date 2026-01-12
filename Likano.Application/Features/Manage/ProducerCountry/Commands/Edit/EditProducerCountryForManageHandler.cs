using Likano.Application.Interfaces;
using MediatR;

namespace Likano.Application.Features.Manage.ProducerCountry.Commands.Edit
{
    public class EditProducerCountryForManageHandler : IRequestHandler<EditProducerCountryForManageCommand, EditProducerCountryForManageResponse>
    {
        readonly IManageRepository _repository;
        public EditProducerCountryForManageHandler(IManageRepository repository)
        {
            _repository = repository;
        }

        public async Task<EditProducerCountryForManageResponse> Handle(EditProducerCountryForManageCommand request, CancellationToken cancellationToken)
        {
            if (request.Id <= 0 || string.IsNullOrWhiteSpace(request.Name))
            {
                return new EditProducerCountryForManageResponse
                {
                    Success = false,
                    StatusCode = 400,
                    Message = "Invalid data"
                };
            }

            var result = await _repository.EditProducerCountry(request.Id, request.Name.Trim());
            return new EditProducerCountryForManageResponse
            {
                Success = result,
                StatusCode = result ? 200 : 404,
                Message = result ? "მწარმოებელი ქვეყანა წარმატებით დარედაქტირდა" : "მონაცემები ვერ მოიძებნა"
            };
        }
    }
}