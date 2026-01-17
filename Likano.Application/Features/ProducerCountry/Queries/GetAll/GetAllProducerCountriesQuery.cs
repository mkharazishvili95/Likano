using Likano.Application.Common.Models;
using MediatR;

namespace Likano.Application.Features.ProducerCountry.Queries.GetAll
{
    public class GetAllProducerCountriesQuery : IRequest<GetAllProducerCountriesResponse>
    {
        public Pagination Pagination { get; set; } = new Pagination();
        public string? SearchString { get; set; }
        public int? Id { get; set; }
    }
}
