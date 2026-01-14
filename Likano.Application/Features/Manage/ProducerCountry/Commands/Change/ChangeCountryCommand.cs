using MediatR;

namespace Likano.Application.Features.Manage.ProducerCountry.Commands.Change
{
    public class ChangeCountryCommand : IRequest<ChangeCountryResponse>
    {
        public int ProductId { get; set; }
        public int NewCountryId { get; set; }
    }
}
