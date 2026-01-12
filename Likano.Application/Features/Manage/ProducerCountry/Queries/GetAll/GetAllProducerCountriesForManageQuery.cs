using Likano.Application.Common.Models;
using MediatR;

namespace Likano.Application.Features.Manage.ProducerCountry.Queries.GetAll
{
    public class GetAllProducerCountriesForManageQuery : IRequest<GetAllProducerCountriesForManageResponse>
    {
        public Pagination Pagination { get; set; } = new Pagination();
        public int? Id { get; set; }
        public string? Name { get; set; }
    }
}