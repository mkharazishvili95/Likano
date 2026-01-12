using MediatR;

namespace Likano.Application.Features.Manage.ProducerCountry.Commands.Edit
{
    public class EditProducerCountryForManageCommand : IRequest<EditProducerCountryForManageResponse>
    {
        public int Id { get; set; }
        public string Name { get; set; } = default!;
    }
}