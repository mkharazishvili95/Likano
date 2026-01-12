using Likano.Application.Interfaces;
using MediatR;

namespace Likano.Application.Features.Manage.ProducerCountry.Commands.Delete
{
    public class DeleteProducerCountryForManageHandler : IRequestHandler<DeleteProducerCountryForManageCommand, DeleteProducerCountryForManageResponse>
    {
        readonly IManageRepository _repository;
        public DeleteProducerCountryForManageHandler(IManageRepository repository)
        {
            _repository = repository;
        }

        public async Task<DeleteProducerCountryForManageResponse> Handle(DeleteProducerCountryForManageCommand request, CancellationToken cancellationToken)
        {
            if (request.Id <= 0)
            {
                return new DeleteProducerCountryForManageResponse
                {
                    Success = false,
                    StatusCode = 400,
                    Message = "Invalid id"
                };
            }

            var ok = await _repository.DeleteProducerCountry(request.Id);
            return new DeleteProducerCountryForManageResponse
            {
                Success = ok,
                StatusCode = ok ? 200 : 404,
                Message = ok ? "მწარმოებელი ქვეყანა წარმატებით წაიშალა" : "მოხდა შეცდომა"
            };
        }
    }
}