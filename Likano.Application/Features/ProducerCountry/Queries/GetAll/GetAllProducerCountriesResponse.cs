using Likano.Application.Common.Models;

namespace Likano.Application.Features.ProducerCountry.Queries.GetAll
{
    public class GetAllProducerCountriesResponse : BaseResponse
    {
        public int TotalCount { get; set; }
        public List<GetAllProducerCountriesItemsResponse> Items { get; set; } = new();
    }
    public class GetAllProducerCountriesItemsResponse
    {
        public int? Id { get; set; }
        public string? Name { get; set; }
    }
}
