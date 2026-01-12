using Likano.Application.Common.Models;

namespace Likano.Application.Features.Manage.ProducerCountry.Queries.GetAll
{
    public class GetAllProducerCountriesForManageResponse : BaseResponse
    {
        public int TotalCount { get; set; }
        public List<GetAllProducerCountriesForManageItemsResponse> Items { get; set; } = new();
    }

    public class GetAllProducerCountriesForManageItemsResponse
    {
        public int Id { get; set; }
        public string Name { get; set; } = default!;
    }
}