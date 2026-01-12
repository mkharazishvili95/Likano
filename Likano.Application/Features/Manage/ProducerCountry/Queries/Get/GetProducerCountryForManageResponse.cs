using Likano.Application.Common.Models;

namespace Likano.Application.Features.Manage.ProducerCountry.Queries.Get
{
    public class GetProducerCountryForManageResponse : BaseResponse
    {
        public int? Id { get; set; }
        public string? Name { get; set; }
    }
}