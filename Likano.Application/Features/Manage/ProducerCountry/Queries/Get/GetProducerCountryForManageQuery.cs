using MediatR;

namespace Likano.Application.Features.Manage.ProducerCountry.Queries.Get
{
    public class GetProducerCountryForManageQuery : IRequest<GetProducerCountryForManageResponse>
    {
        public int Id { get; set; }
    }
}