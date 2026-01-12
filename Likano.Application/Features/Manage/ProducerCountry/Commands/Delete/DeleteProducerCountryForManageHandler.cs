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

            var producerCountry = await _repository.GetProducerCountry(request.Id);

            if(producerCountry == null)
                return new DeleteProducerCountryForManageResponse { Message = "მწარმოებელი ქვეყანა არ მოიძებნა", Success = false, StatusCode = 404 };

            var deleted = await _repository.DeleteProducerCountry(request.Id);

            
            return new DeleteProducerCountryForManageResponse 
            { 
                Success = deleted, 
                StatusCode = deleted ? 200 : 500, 
                Message = deleted ? "მწარმოებელი ქვეყანა წარმატებით წაიშალა" : "მოხდა შეცდომა" 
            };
        }
    }
}