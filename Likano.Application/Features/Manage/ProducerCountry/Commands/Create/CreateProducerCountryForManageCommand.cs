using MediatR;

namespace Likano.Application.Features.Manage.ProducerCountry.Commands.Create
{
    public class CreateProducerCountryForManageCommand : IRequest<CreateProducerCountryForManageResponse>
    {
        public string Name { get; set; } = default!;
    }
}