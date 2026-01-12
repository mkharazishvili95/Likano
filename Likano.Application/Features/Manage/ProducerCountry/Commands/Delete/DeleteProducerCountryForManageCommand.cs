using MediatR;

namespace Likano.Application.Features.Manage.ProducerCountry.Commands.Delete
{
    public class DeleteProducerCountryForManageCommand : IRequest<DeleteProducerCountryForManageResponse>
    {
        public int Id { get; set; }
    }
}